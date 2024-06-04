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
    /// Controller for managing customer addresses.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _LT2019context;
        private readonly BetacycleUsersContext _usersContext;

        public CustomerAddressesController(AdventureWorksLt2019Context LTcontext, BetacycleUsersContext usersContext)
        {
            _LT2019context = LTcontext;
            _usersContext = usersContext;
        }

        private static Logger CustomerAddressNLogLogger = LogManager.GetCurrentClassLogger();

        // GET: api/CustomerAddresses
        [HttpGet]
        /// <summary>
        /// Retrieves all customer addresses.
        /// </summary>
        public async Task<ActionResult<IEnumerable<CustomerAddress>>> GetCustomerAddresses()
        {
            return await _LT2019context.CustomerAddresses.ToListAsync();
        }

        // GET: api/CustomerAddresses/5
        [HttpGet("{id}")]
        /// <summary>
        /// Retrieves a customer address by its ID.
        /// </summary>
        /// <param name="id">The ID of the customer address to retrieve.</param>
        public async Task<ActionResult<CustomerAddress>> GetCustomerAddress(int id)
        {
            var customerAddress = await _LT2019context.CustomerAddresses.FindAsync(id);

            if (customerAddress == null)
            {
                return NotFound();
            }

            return customerAddress;
        }

        // GET: api/CustomerAddresses/Customer/'n'
        [HttpGet("Customer/{custId}")]
        /// <summary>
        /// Retrieves a customer address by customer ID.
        /// </summary>
        /// <param name="custId">The ID of the customer.</param>
        public async Task<ActionResult<CustomerAddressFrontEnd>> GetCustomerAddressByCustId(int custId)
        {
            var customerAddress = await _LT2019context.CustomerAddresses.FirstOrDefaultAsync(c => c.CustomerId == custId);

            if (customerAddress == null)
            {
                CustomerAddressNLogLogger.Info("CustomerAddressesController - GetCustomerAddressByCustId - Address Not Found");
                return NotFound();
            }

            CustomerAddressNLogLogger.Info("CustomerAddressesController - GetCustomerAddressByCustId - Sending Address Data to FrontEnd");
            CustomerAddressFrontEnd customerAddressFE = new CustomerAddressFrontEnd
            {
                CustomerId = customerAddress.CustomerId,
                AddressId = customerAddress.AddressId,
                AddressType = customerAddress.AddressType
            };

            return customerAddressFE;
        }

        // GET: api/CustomerAddresses/Address/'n'
        [HttpGet("Address/{adrsId}")]
        /// <summary>
        /// Retrieves a customer address by its address ID.
        /// </summary>
        /// <param name="adrsId">The ID of the address.</param>
        public async Task<ActionResult<CustomerAddressFrontEnd>> GetCustomerAddressByAdrsId(int adrsId)
        {
            var customerAddress = await _LT2019context.CustomerAddresses.FirstOrDefaultAsync(a => a.AddressId == adrsId);

            if (customerAddress == null)
            {
                CustomerAddressNLogLogger.Info("CustomerAddressesController - GetCustomerAddressByAdrsId - Address Not Found");
                return NotFound();
            }

            CustomerAddressNLogLogger.Info("CustomerAddressesController - GetCustomerAddressByAdrsId - Sending Address Data to FrontEnd");
            CustomerAddressFrontEnd customerAddressFE = new CustomerAddressFrontEnd
            {
                CustomerId = customerAddress.CustomerId,
                AddressId = customerAddress.AddressId,
                AddressType = customerAddress.AddressType
            };

            return customerAddressFE;
        }

        // PUT: api/CustomerAddresses/5
        [HttpPut("{id}")]
        /// <summary>
        /// Updates a customer address.
        /// </summary>
        /// <param name="id">The ID of the customer address to update.</param>
        /// <param name="customerAddress">The updated customer address data.</param>
        public async Task<IActionResult> PutCustomerAddress(int id, CustomerAddress customerAddress)
        {
            if (id != customerAddress.CustomerId)
            {
                CustomerAddressNLogLogger.Error("CustomerAddressesController - PutCustomerAddress - Id Not Found");
                return BadRequest();
            }

            _LT2019context.Entry(customerAddress).State = EntityState.Modified;

            try
            {
                await _LT2019context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAddressExists(id))
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

        // PUT: api/CustomerAddresses/5
        [HttpPut("Customer/{custId}")]
        /// <summary>
        /// Updates a customer address by customer ID.
        /// </summary>
        /// <param name="custId">The ID of the customer.</param>
        /// <param name="customerAddressFE">The updated customer address data.</param>
        public async Task<IActionResult> PutCustomerAddressByCustId(int custId, CustomerAddressFrontEnd customerAddressFE)
        {
            if (custId != customerAddressFE.CustomerId)
            {
                return BadRequest();
            }

            var customerExistsInOldDb = await _LT2019context.Customers.AnyAsync(c => c.CustomerId == customerAddressFE.CustomerId);

            if (!customerExistsInOldDb)
            {
                var userExistsInNewDb = await _usersContext.Users.AnyAsync(u => u.Id == customerAddressFE.CustomerId);

                if (!userExistsInNewDb)
                {
                    return NotFound("CustomerId not found in either Customers or Users tables.");
                }
            }

            var customerAddress = await _LT2019context.CustomerAddresses
                                                .FirstOrDefaultAsync(ca => ca.CustomerId == custId);

            if (customerAddress == null)
            {
                return NotFound();
            }

            customerAddress.AddressType = customerAddressFE.AddressType;
            customerAddress.ModifiedDate = DateTime.Now;

            try
            {
                await _LT2019context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerAddressExists(custId))
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

        // POST: api/CustomerAddresses
        [HttpPost]
        /// <summary>
        /// Creates a new customer address.
        /// </summary>
        /// <param name="customerAddress">The customer address to add.</param>
        public async Task<ActionResult<CustomerAddress>> PostCustomerAddress(CustomerAddress customerAddress)
        {
            _LT2019context.CustomerAddresses.Add(customerAddress);
            try
            {
                await _LT2019context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerAddressExists(customerAddress.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomerAddress", new { id = customerAddress.CustomerId }, customerAddress);
        }

        // POST: api/CustomerAddresses/FrontEnd
        [HttpPost("FrontEnd")]
        /// <summary>
        /// Creates a new customer address from front-end data.
        /// </summary>
        /// <param name="customerAddressFE">The customer address data from the front end.</param>
        public async Task<ActionResult<CustomerAddress>> PostCustomerAddressFrontEnd(CustomerAddressFrontEnd customerAddressFE)
        {
            var customerExistsInOldDb = await _LT2019context.Customers.AnyAsync(c => c.CustomerId == customerAddressFE.CustomerId);

            if (!customerExistsInOldDb)
            {
                var userExistsInNewDb = await _usersContext.Users.AnyAsync(u => u.Id == customerAddressFE.CustomerId);

                if (!userExistsInNewDb)
                {
                    return NotFound("CustomerId not found in either Customers or Users tables.");
                }
            }

            CustomerAddress customerAddress = new CustomerAddress
            {
                AddressId = customerAddressFE.AddressId,
                CustomerId = customerAddressFE.CustomerId,
                AddressType = customerAddressFE.AddressType
            };

            _LT2019context.CustomerAddresses.Add(customerAddress);
            try
            {
                await _LT2019context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CustomerAddressExists(customerAddress.CustomerId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCustomerAddress", new { id = customerAddress.CustomerId }, customerAddress);
        }

        // DELETE: api/CustomerAddresses/5
        [HttpDelete("{id}")]
        /// <summary>
        /// Deletes a customer address by ID.
        /// </summary>
        /// <param name="id">The ID of the customer address to delete.</param>
        public async Task<IActionResult> DeleteCustomerAddress(int id)
        {
            var customerAddress = await _LT2019context.CustomerAddresses.FindAsync(id);
            if (customerAddress == null)
            {
                return NotFound();
            }

            _LT2019context.CustomerAddresses.Remove(customerAddress);
            await _LT2019context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerAddressExists(int id)
        {
            return _LT2019context.CustomerAddresses.Any(e => e.CustomerId == id);
        }
    }
}

