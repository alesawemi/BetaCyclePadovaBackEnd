using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;
using NLog;

namespace BetaCycle_Padova.Controllers.LTWorks
{
    /// <summary>
    /// Controller for handling addresses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public AddressesController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        private static Logger AddressNlogLogger = LogManager.GetCurrentClassLogger();

        // GET: api/Addresses
        [HttpGet]
        /// <summary>
        /// Retrieves all addresses.
        /// </summary>
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        /// <summary>
        /// Retrieves an address by its ID.
        /// </summary>
        /// <param name="id">The ID of the address to retrieve.</param>
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            return address;
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        /// <summary>
        /// Updates an existing address.
        /// </summary>
        /// <param name="id">The ID of the address to update.</param>
        /// <param name="address">The updated address data.</param>
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressId)
            {
                return BadRequest();
            }

            _context.Entry(address).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(id))
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

        // PUT: api/Addresses/5
        [HttpPut("FrontEnd/{adrsId}")]
        /// <summary>
        /// Updates an address from the frontend.
        /// </summary>
        /// <param name="adrsId">The ID of the address to update.</param>
        /// <param name="address">The updated address data.</param>
        public async Task<IActionResult> PutAddressFrontEnd(int adrsId, Address address)
        {
            AddressNlogLogger.Info("AddressesController - PutAddressFrontEnd");

            if (adrsId != address.AddressId)
            {
                AddressNlogLogger.Error("AddressesController - PutAddressFrontEnd - AddressId not found");
                return BadRequest();
            }

            // Find the existing entity in the database
            var existingAddress = await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == adrsId);

            if (existingAddress == null)
            {
                AddressNlogLogger.Error("AddressesController - PutAddressFrontEnd - NULL Address");
                return NotFound();
            }

            existingAddress.AddressLine1 = address.AddressLine1;
            existingAddress.AddressLine2 = address.AddressLine2;
            existingAddress.City = address.City;
            existingAddress.StateProvince = address.StateProvince;
            existingAddress.CountryRegion = address.CountryRegion;
            existingAddress.PostalCode = address.PostalCode;

            existingAddress.ModifiedDate = DateTime.Now;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AddressExists(adrsId))
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

        // POST: api/Addresses
        [HttpPost]
        /// <summary>
        /// Creates a new address.
        /// </summary>
        /// <param name="address">The address data to create.</param>
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAddress", new { id = address.AddressId }, address);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        /// <summary>
        /// Deletes an address by its ID.
        /// </summary>
        /// <param name="id">The ID of the address to delete.</param>
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.AddressId == id);
        }
    }
}
