using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.Users;
using BetaCycle_Padova.Models.LTWorks;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net;
using NLog;

namespace BetaCycle_Padova.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BetacycleUsersContext _context;

        public UsersController(BetacycleUsersContext context)
        {
            _context = context;
        }

        private static Logger UserNlogLogger = LogManager.GetCurrentClassLogger();

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        //nuovo controller x ricerca email nel db 
        [HttpGet("email/{emailAddress}")]
        public async Task<ActionResult<User>> GetUserByEmail(string emailAddress)
        {
            var user = await _context.Users.FirstOrDefaultAsync(c => c.Mail == emailAddress);

            if (user == null)
            {
                UserNlogLogger.Info("UserController - GetUserByEmail - NotFound");
                return NotFound();
            }

            UserNlogLogger.Info("UserController - GetUserByEmail - Ok");
            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {

            try
            {
                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch (DbUpdateException dbex)
            {
                if (UserExists(user.Id))
                {
                    UserNlogLogger.Error(dbex, "UserController - PostUser - UserExists, Conflict");
                    return Conflict(); //CAPIRE COSA TI RESTITUISCE
                }
                else
                {
                    Console.WriteLine("PROBLEMA NEL POST USER: " + dbex.Message);
                    UserNlogLogger.Error(dbex, "UserController - PostUser - BadRequest");
                    return BadRequest(dbex);
                }
            }


            //return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("StoredP")]
        public async Task<ActionResult<User>> PostUserSP(User user)
        {

            try
            {
                _context.Users.Add(user);

                // Chiama la stored procedure per l'inserimento dei dati
                await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC migration @IdOld = {user.Id}, @Name={user.Name}, @Surname={user.Surname}, @Phone={user.Phone}, @Mail={user.Mail}, @PasswordHash={user.Credential.Password}, @PasswordSalt={user.Credential.Salt}, @UserId = {user.Id}");            

                return CreatedAtAction("GetUser", new { id = user.Id }, user);
            }
            catch (DbUpdateException dbex)
            {
                if (UserExists(user.Id))
                {
                    UserNlogLogger.Error(dbex, "UserController - PostUser - UserExists, Conflict");
                    return Conflict(); //CAPIRE COSA TI RESTITUISCE
                }
                else
                {
                    Console.WriteLine("PROBLEMA NEL POST USER: " + dbex.Message);
                    UserNlogLogger.Error(dbex, "UserController - PostUser - BadRequest");
                    return BadRequest(dbex);
                }
            }
            

            //return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        
        // POST: api/Users/Registration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Registration")]
        public async Task<ActionResult<User>> PostRegistration(RegisterUser rUser)
        {
            //Console.WriteLine("RegistrationUser: " + rUser.FirstName + " - " + rUser.LastName + " - " + rUser.Phone + " - " + rUser.EmailAddress);
            string pass = Encoding.UTF8.GetString(Convert.FromBase64String(rUser.Password)); //password in chiaro

            //Console.WriteLine("Password in chiaro: "+pass);
            byte[] salt = new byte[32];
            RandomNumberGenerator.Fill(salt);

            string passHash = Convert.ToBase64String(
                                    KeyDerivation.Pbkdf2(
                                    password: pass,
                                    salt: salt,
                                    prf: KeyDerivationPrf.HMACSHA256,
                                    iterationCount: 100000, // Modificare se necessario
                                    numBytesRequested: 16));
            string passSalt = Convert.ToBase64String(salt);

            //Console.WriteLine("PassHash: "+passHash);
            //Console.WriteLine("PassSalt: " + passSalt);

            Credential newCredential = new Credential
            { 
                Password = passHash,
                Salt = passSalt
            };

            //Console.WriteLine("Credential: " + newCredential.Password + " - " + newCredential.Salt);


            User user = new User
            {
                Name = rUser.FirstName,
                Surname = rUser.LastName,
                Phone = rUser.Phone,
                Mail = rUser.EmailAddress,                
            };
            user.Credential = newCredential;
       
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
                UserNlogLogger.Info("UserController - PostRegistration - Ok");
                return Ok(new { status = HttpStatusCode.OK });
            }
            catch (DbUpdateException dbex)
            {
                if (UserExists(user.Mail))
                {
                    UserNlogLogger.Error(dbex, "UserController - PostRegistration - EmailExists - Conflict");
                    return Conflict(); //RESTITUISCE UN 409    new { status = HttpStatusCode.Conflict }
                }
                else
                {
                    UserNlogLogger.Error(dbex, "UserController - PostRegistration - InternalError");
                    return StatusCode(500, new { status = HttpStatusCode.InternalServerError }); // Gestisci altri casi di errore e restituisci il codice di stato appropriato
                }
            }
        }        

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        private bool UserExists(string email)
        {
            return _context.Users.Any(e => e.Mail == email);
        }
    }
}
