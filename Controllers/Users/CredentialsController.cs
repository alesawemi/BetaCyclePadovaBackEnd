using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.Users;
using NLog;

namespace BetaCycle_Padova.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class CredentialsController : ControllerBase
    {
        private readonly BetacycleUsersContext _context;

        public CredentialsController(BetacycleUsersContext context)
        {
            _context = context;
        }

        private static Logger CredentialNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves all credentials.
        /// </summary>
        /// <returns>A list of credentials.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Credential>>> GetCredentials()
        {
            return await _context.Credentials.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific credential by ID.
        /// </summary>
        /// <param name="id">The ID of the credential.</param>
        /// <returns>The credential with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Credential>> GetCredential(int id)
        {
            var credential = await _context.Credentials.FindAsync(id);

            if (credential == null)
            {
                return NotFound();
            }

            return credential;
        }

        /// <summary>
        /// Retrieves a credential by the user ID.
        /// </summary>
        /// <param name="userId">The user ID to filter by.</param>
        /// <returns>The credential associated with the specified user ID.</returns>
        [HttpGet("userId/{userId}")]
        public async Task<ActionResult<Credential>> GetCredentialByUserId(int userId)
        {
            var credentials = await _context.Credentials.FirstOrDefaultAsync(c => c.IdUser == userId);

            if (credentials == null)
            {
                CredentialNlogLogger.Info("CredentialController - GetCredentialByUserId - NotFound");
                return NotFound();
            }

            CredentialNlogLogger.Info("CredentialController - GetCredentialByUserId - Ok");
            return credentials;
        }

        /// <summary>
        /// Updates a specific credential.
        /// </summary>
        /// <param name="id">The ID of the credential to update.</param>
        /// <param name="credential">The updated credential.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCredential(int id, Credential credential)
        {
            if (id != credential.Id)
            {
                return BadRequest();
            }

            _context.Entry(credential).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CredentialExists(id))
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

        /// <summary>
        /// Creates a new credential.
        /// </summary>
        /// <param name="credential">The credential to create.</param>
        /// <returns>The created credential.</returns>
        [HttpPost]
        public async Task<ActionResult<Credential>> PostCredential(Credential credential)
        {
            try
            {
                _context.Credentials.Add(credential);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCredential", new { id = credential.Id }, credential);
            }
            catch (DbUpdateException)
            {
                if (CredentialExists(credential.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes a specific credential.
        /// </summary>
        /// <param name="id">The ID of the credential to delete.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCredential(int id)
        {
            var credential = await _context.Credentials.FindAsync(id);
            if (credential == null)
            {
                return NotFound();
            }

            _context.Credentials.Remove(credential);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a credential exists.
        /// </summary>
        /// <param name="id">The ID of the credential to check.</param>
        /// <returns>True if the credential exists, false otherwise.</returns>
        private bool CredentialExists(int id)
        {
            return _context.Credentials.Any(e => e.Id == id);
        }
    }
}
