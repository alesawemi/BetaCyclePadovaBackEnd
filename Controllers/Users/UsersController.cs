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
                return NotFound();
            }

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
            _context.Users.Add(user);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbex)
            {
                if (UserExists(user.Id))
                {                    
                    return Conflict(); //CAPIRE COSA TI RESTITUISCE
                }
                else
                {
                    Console.WriteLine("PROBLEMA NEL POST USER: " + dbex.Message);
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }


        
        // POST: api/Users/Registration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Registration")]
        public async Task<ActionResult<User>> PostRegistration(RegisterUser rUser)
        {
            Console.WriteLine("RegistrationUser: " + rUser.FirstName + " - " + rUser.LastName + " - " + rUser.Phone + " - " + rUser.EmailAddress);
            string pass = Encoding.UTF8.GetString(Convert.FromBase64String(rUser.Password)); //password in chiaro

            Console.WriteLine("Password in chiaro: "+pass);
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

            Console.WriteLine("PassHash: "+passHash);
            Console.WriteLine("PassSalt: " + passSalt);

            Credential newCredential = new Credential
            { 
                Password = passHash,
                Salt = passSalt
            };

            Console.WriteLine("Credential: " + newCredential.Password + " - " + newCredential.Salt);


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
                Console.WriteLine("Entro nel try");

                await _context.SaveChangesAsync();

                Console.WriteLine("Ho Finito nel try");
            }
            catch (DbUpdateException dbex)
            {
                if (UserExists(user.Id))
                {
                    Console.WriteLine("CONFLICT");
                    return Conflict(); //CAPIRE COSA TI RESTITUISCE
                }
                else
                {
                    Console.WriteLine("PROBLEMA NEL POST REGISTRA USER: " + dbex.Message);
                }
            }

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
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
    }
}
