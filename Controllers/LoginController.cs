using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using BetaCycle_Padova.BLogic.Authentication.Basic;
using BetaCycle_Padova.Models;
using Microsoft.Identity.Client;
using BetaCycle_Padova.BLogic.Authentication;
using BetaCycle_Padova.Models.LTWorks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using BetaCycle_Padova.Controllers.LTWorks;
using BetaCycle_Padova.Controllers.Users;
using BetaCycle_Padova.Models.Users;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using NLog;
using Logger = NLog.Logger;
using WebAca5CodeFirst.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BetaCycle_Padova.Controllers
{
    [ApiController] //attributo
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        // ! ricorda di aggiungere nel program.cs AddScoped in #region BUILDER
        private readonly OldCustomersController _customersController;
        private readonly UsersController _usersController;
        private readonly CredentialsController _credentialsController;

        private JwtSettings _jwtSettings;

        private static Logger LoginNlogLogger = LogManager.GetCurrentClassLogger(); // istanzio il mio Logger qui

        public LoginController (OldCustomersController customersController, UsersController usersController, 
            CredentialsController credentialsController, JwtSettings jwtSettings)
        {
            _customersController = customersController;
            _usersController = usersController;
            _credentialsController = credentialsController;

            _jwtSettings = jwtSettings;
        }


        [HttpPost]
        //IActionResult definisce una sorta di regola per cui io ti ritorno risultati specifici legati al metodo - non sono altro che le risposta HTTP tipiche

        //public async Task<IActionResult> Login(string inputUsername, string InputPassword)
        public async Task<IActionResult> Login(LoginCredentials credentials) {
        //{ LoginCredentials credentials = new LoginCredentials();
        //    credentials.Username = inputUsername;
        //    credentials.Password = InputPassword;

            // usiamo post perché è metodo http che accetta credenziali 
            // poi qui dentro facciamo tutti i nostri controlli
            // questo è il FLOW:
            // (A) controllo "nuovo" db --> (B) se c'è email controlla psw --> (C) LoginOk/PswErrata
            // (D) se non c'è email allora controllo "vecchio" db --> (E) se c'è email controlla psw --> (F) LoginOk/PswErrata
            // (G) se è tutto ok fai migrazione a nuovo db 
            // (H) se non c'è email --> NotFound e FrontEnd redirect a Registrati --> sarà un Post con le info da salvare nei db

            // !! MANCA REGISTRAZIONE --> HttpPost x OldCustomer in "vecchio" db e User in "nuovo" db
            // --> si possono usare direttamente post in frontend??

            //faccio tutti i miei controlli,  ci sta gestire nel DB User una tabella di Log per gli errors 
            //E' giusto che l'admin o il tecnico possa intervenire capendo il tipo di errore
            //Intercettare anche accessi abusivi ? 

            LoginNlogLogger.Info("LoginController - Received login request.");            

            try
            {
                //facciamo ulteriore controllo perché questo check c'è già nel FrontEnd
                if ( (credentials.Username != "" && credentials.Password != ""))
                {                                      
                    // (A)                    
                    var foundUser = FindEmailNew(credentials.Username); // controllo email nel db nuovo 

                    #region email NON è nel nuovo db
                    //if (foundUser == null) // (D)
                    if (foundUser.Result.Value == null)
                    {
                        var foundCustomer = FindEmailOld(credentials.Username); // controllo email nel db vecchio 

                        #region email NON è nel vecchio db --> registrazione
                        if (foundCustomer.Result.Value == null)
                        {
                            LoginNlogLogger.Error("Login Controller - L'utente non è ancora registrato - reindirizzamento alla page Registrati.");
                            return NotFound(); // (H) //frontend che intercetta questo NotFound deve reindirizzare utente alla pag di registrazione
                        }
                        #endregion

                        #region email è nel vecchio db 
                        else
                        {
                            #region controllo psw e invio risposta al frontend : LoginOk/Credenziali Non Corrette + migrazione 
                            // (E) se email trovata nel "vecchio" database --> controllo password
                            var passwordInDBHash = foundCustomer.Result.Value.PasswordHash;
                            var passwordInDBSalt = foundCustomer.Result.Value.PasswordSalt;

                            if (VerifyPassword(credentials.Password, passwordInDBSalt, passwordInDBHash))
                            {
                                LoginNlogLogger.Info("LoginController - Customer esiste nel vecchio db - Password OK - Migrazione in corso");
                                #region migrazione
                                // (G) se è tutto ok fai migrazione a nuovo db
                                Models.Users.Credential newCredential = new Models.Users.Credential
                                    (
                                        foundCustomer.Result.Value.PasswordHash,
                                        foundCustomer.Result.Value.PasswordSalt
                                        //non scelgo io UserId perché in tab Credentials sarà foreignKey che collega a User, quindi deve prendere UserId da newUser in tab User
                                    );

                                User newUser = new User
                                {
                                    Id = foundCustomer.Result.Value.CustomerId,
                                    Name = foundCustomer.Result.Value.FirstName,
                                    Surname = foundCustomer.Result.Value.LastName,
                                    Phone = foundCustomer.Result.Value.Phone,
                                    Mail = foundCustomer.Result.Value.EmailAddress,
                                   // OldCustomerId = foundCustomer.Result.Value.CustomerId
                                };

                                newUser.Credential = newCredential;
                                //con PostUser newCredential in tab Credentials prendere Id da newUser in tab User (spero? -Marti)
                                var migrateUser = await _usersController.PostUserSP(newUser); //NICHOLAS 17/05/2024 - uso di storedProcedure

                                

                                // Console.WriteLine(migrateUser.Result.GetType()); // Microsoft.AspNetCore.Mvc.CreatedAtActionResult

                                // http post nello User Controller ritorna un CreatedAtAction quando la migrazione ha successo
                                // ed è difficile accedere alle proprietà di questo tipo per fare qui un controllo sul risultato della migrazione

                                if (migrateUser.Result == BadRequest())
                                {
                                    LoginNlogLogger.Error("Login Controller - Problema con la Migrazione!");
                                    return BadRequest(new { message = "Problema con la Migrazione!" }); // (F) 
                                    // per ora lasciamo questo messaggio di errore per noi per capire se c'è un problema con la migrazione
                                    // ma poi andrà tolto/cambiato, cosa se ne fa utente di questa info?
                                    // si potrebbe pensare di restituire un "errore imprevisto, si prega di riprovare più tardi"
                                    // e notificare il problema a chi gestisce il sito così che possano intervenire --> ci sarà log error x aiutare
                                }
                                #endregion

                                else
                                {
                                    LoginNlogLogger.Info("LoginController - Migrazione completata!");

                                    // email e password corrette --> genero token e lo includo nel return come overload di Ok()

                                    LoginNlogLogger.Info("LoginController - Generate Jwt Token  (vecchio db)");

                                    var role = "user"; // esempio, potresti dover estrarre il ruolo da qualche parte
                                    var token = GenerateJwtToken(credentials.Username, role);

                                    return Ok(new {token}); // (F)
                                }
                            }
                            else
                            {
                                LoginNlogLogger.Error("LoginController - Credenziali Non Corrette (vecchio db)");
                                return BadRequest(new { message = "Credenziali Non Corrette" }); // (F)
                            }
                            #endregion
                        }
                        #endregion
                    }
                    #endregion

                    #region email è nel nuovo db --> controllo psw e invio risposta al frontend : LoginOk/Credenziali Non Corrette 
                    else
                    {
                        LoginNlogLogger.Info("LoginController - User trovato nel nuovo database --> controllo password");
                        // (B) se email trovata nel "nuovo" database --> controllo password

                        // get User by Email non riempe anche il campo Credential di user --> serve get dalla tab Credentials

                        var foundCredentials = FindCredentialNew(foundUser.Result.Value.Id);

                        if (foundCredentials.Result.Value == null)
                        {
                            LoginNlogLogger.Fatal("LoginController - Fatal Error: nel nuovo db c'è email in User ma non ci sono Credential corrispondenti");
                            return BadRequest(new { message = "Fatal Error" }); // ha trovato email nel nuovo db ma non c'è password associata
                        }

                        var passwordInDBHash = foundCredentials.Result.Value.Password;
                        var passwordInDBSalt = foundCredentials.Result.Value.Salt;

                        if (VerifyPassword(credentials.Password, passwordInDBSalt, passwordInDBHash))
                        {
                            LoginNlogLogger.Info("LoginController - Credenziali corrette (nuovo db)");

                            // email e password corrette --> genero token e lo includo nel return come overload di Ok()

                            LoginNlogLogger.Info("LoginController - Generate Jwt Token (nuovo db)");

                            var role = "user"; 
                            if (foundUser.Result.Value.Role == "admin")
                                {
                                    role = "admin";
                                }

                            var token = GenerateJwtToken(credentials.Username, role);



                            return Ok(new { token }); // (C)
                        }
                        else
                        {
                            LoginNlogLogger.Error("LoginController - Credenziali Non Corrette(nuovo db)");
                            return BadRequest(new { message = "Credenziali Non Corrette" }); // (C)
                        }
                    }
                    #endregion

                }
                else
                {                   
                    LoginNlogLogger.Error("LoginController - Credenziali Non Fornite");
                    return BadRequest(new { message = "Credenziali Non Fornite" });
                }
            }
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController ha sollevato un'eccezione nel catch");           
            }

            LoginNlogLogger.Error("LoginController - Errore Grave nel Login.");            
            return BadRequest();
        }

        #region metodo x controllare se password è nel nuovo db - tab Credentials
        private async Task<ActionResult<Models.Users.Credential>> FindCredentialNew(int id)
        {
            try
            {
                var credentialResult = await _credentialsController.GetCredentialByUserId(id);

                if (credentialResult.Value == null) return NotFound();

                return credentialResult;
            }
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController - FindCredentialNew");
            }

            return BadRequest();
        }
        #endregion

        #region metodo x controllare se email è nel nuovo db - tab User
        private async Task<ActionResult<User>> FindEmailNew(string email)
        {
            try
            {
                var userResult = await _usersController.GetUserByEmail(email);

                if (userResult.Value == null) return NotFound();

                return userResult;
            }
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController - FindEmailNew");
            }

            return BadRequest();
        }
        #endregion

        #region metodo x controllare se email è nel vecchio db
        private async Task<ActionResult<Customer>> FindEmailOld(string email)
        {
            try
            {
                var customerResult = await _customersController.GetCustomerByEmail(email);

                if (customerResult.Value == null) return NotFound();
                
                return customerResult;
            } 
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController - FindEmailOld");
            }

            return BadRequest();    
        }
        #endregion

        #region metodo x verifica della password 
        private bool VerifyPassword(string password, string saltEncrypt, string encryptedPassword)
        {                               // clear psw, salt nel db, psw nel db
            try
            {
                // Converti il salt da stringa a un array di byte
                byte[] salt = Convert.FromBase64String(saltEncrypt);
                
                //Console.WriteLine("Salt recuperato durante il verify:");
                //Console.WriteLine(BitConverter.ToString(salt)); // Stampa il salt in formato esadecimale
                
                // Calcola la password criptata utilizzando la password inserita e lo stesso salt
                string enteredPasswordEncrypted = Convert.ToBase64String(
                    KeyDerivation.Pbkdf2(
                        password: password,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,

                        ////Encrypt di Nicholas
                        //iterationCount: 10000, // Modificare se necessario
                        //numBytesRequested: 32)); // Lunghezza della password criptata

                        //Encrypt di Marti
                        iterationCount: 100000,
                        numBytesRequested: 16));

                        Console.WriteLine("La psw inserita criptata con il salt preso dal DB è: " + enteredPasswordEncrypted);
                        Console.WriteLine("La psw criptata contenuta nel DB è: " + encryptedPassword);

                        // Confronta la password criptata appena calcolata con quella memorizzata
                        return enteredPasswordEncrypted.Equals(encryptedPassword);
            }
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController - VerifyPassword");
                return false;
            }           
            
        }
        #endregion 


        #region metodo x generare il token Jwt
        private string GenerateJwtToken(string username,string role)
        {
            try
            {
                var secretKey = _jwtSettings.SecretKey;
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secretKey); //qnd in program.cs arriva token si farà confronto tra qll che ho passato x generare token e qll che autenticazione si aspetta di trovare in token per dare l'ok
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    //parametri che esistono grazie a SecurityTokenDescriptor = subject, issuer etc
                    Subject = new ClaimsIdentity(new[] //stiamo costruendo info fondamentali che andranno salvate nel token
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, role)
                    }),
                    Expires = DateTime.Now.AddMinutes(_jwtSettings.ExpirationMinutes),
                    Issuer = _jwtSettings.Issuer,
                    Audience = _jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                string tokenString = tokenHandler.WriteToken(token);

                return tokenString;
            }
            catch (Exception ex)
            {
                LoginNlogLogger.Error(ex, "LoginController - GenerateJwtToken");
                return "ERROR";
            }            
        }
        #endregion

    }
}
