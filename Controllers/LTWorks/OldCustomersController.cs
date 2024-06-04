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
    [Route("api/[controller]")]
    [ApiController]
    public class OldCustomersController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public OldCustomersController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        private static Logger CustomerNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Retrieves all customers.
        /// </summary>
        /// <returns>A list of customers.</returns>
        // GET: api/OldCustomers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        /// <summary>
        /// Retrieves a specific customer by ID.
        /// </summary>
        /// <param name="id">The ID of the customer.</param>
        /// <returns>The customer with the specified ID.</returns>
        // GET: api/OldCustomers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        /// <summary>
        /// Retrieves a specific customer by email address.
        /// </summary>
        /// <param name="emailAddress">The email address of the customer.</param>
        /// <returns>The customer with the specified email address.</returns>
        [HttpGet("email/{emailAddress}")]
        public async Task<ActionResult<Customer>> GetCustomerByEmail(string emailAddress)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.EmailAddress == emailAddress);

            if (customer == null)
            {
                CustomerNlogLogger.Info("OldCustomerController - GetCustomerByEmail - NotFound");
                return NotFound();
            }

            CustomerNlogLogger.Info("OldCustomerController - GetCustomerByEmail - Ok");
            return customer;
        }

        /// <summary>
        /// Updates a specific customer by ID.
        /// </summary>
        /// <param name="id">The ID of the customer to update.</param>
        /// <param name="customer">The updated customer object.</param>
        /// <returns>No content if the update is successful.</returns>
        // PUT: api/OldCustomers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
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
        /// Adds a new customer.
        /// </summary>
        /// <param name="customer">The customer to add.</param>
        /// <returns>The newly created customer.</returns>
        // POST: api/OldCustomers
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        /// <summary>
        /// Deletes a specific customer by ID.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        // DELETE: api/OldCustomers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Checks if a customer with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the customer.</param>
        /// <returns>True if the customer exists, otherwise false.</returns>
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}
