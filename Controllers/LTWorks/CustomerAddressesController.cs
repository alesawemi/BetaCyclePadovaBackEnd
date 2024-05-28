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
        public async Task<ActionResult<IEnumerable<CustomerAddress>>> GetCustomerAddresses()
        {
            return await _LT2019context.CustomerAddresses.ToListAsync();
        }

        // GET: api/CustomerAddresses/5
        [HttpGet("{id}")]
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
        public async Task<ActionResult<CustomerAddressFrontEnd>> GetCustomerAddressByCustId(int custId)
        {
            //faccio una ricerca per CustomerId
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
        public async Task<ActionResult<CustomerAddressFrontEnd>> GetCustomerAddressByAdrsId(int adrsId)
        {
            //Faccio una ricerca per AddressId
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Customer/{custId}")]
        public async Task<IActionResult> PutCustomerAddressByCustId(int custId, CustomerAddressFrontEnd customerAddressFE)
        {
            if (custId != customerAddressFE.CustomerId)
            {
                return BadRequest();
            }

            // Verifica se il CustomerId esiste nella tabella Customers del vecchio database
            var customerExistsInOldDb = await _LT2019context.Customers.AnyAsync(c => c.CustomerId == customerAddressFE.CustomerId);

            // Se il CustomerId non esiste nella tabella Customers, verifica se esiste nella tabella Users del nuovo database
            if (!customerExistsInOldDb)
            {
                var userExistsInNewDb = await _usersContext.Users.AnyAsync(u => u.Id == customerAddressFE.CustomerId);

                if (!userExistsInNewDb)
                {
                    return NotFound("CustomerId not found in either Customers or Users tables.");
                }
            }

            // Trova l'entità esistente nel database
            var customerAddress = await _LT2019context.CustomerAddresses
                                                .FirstOrDefaultAsync(ca => ca.CustomerId == custId);

            if (customerAddress == null)
            {
                return NotFound();
            }

            // Aggiorna solo i campi specifici
            // customerAddress.AddressId = customerAddressFE.AddressId; // Questo è commentato, come nell'originale
            customerAddress.AddressType = customerAddressFE.AddressType;
            customerAddress.ModifiedDate = DateTime.Now;

            try
            {
                // Salva i cambiamenti nel database
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
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
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("FrontEnd")]
        public async Task<ActionResult<CustomerAddress>> PostCustomerAddressFrontEnd(CustomerAddressFrontEnd customerAddressFE)
        {
            // Verifica se il CustomerId esiste nella tabella Customers del vecchio database
            var customerExistsInOldDb = await _LT2019context.Customers.AnyAsync(c => c.CustomerId == customerAddressFE.CustomerId);

            // Se il CustomerId non esiste nella tabella Customers, verifica se esiste nella tabella Users del nuovo database
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
