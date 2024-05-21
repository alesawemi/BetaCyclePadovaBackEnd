﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using BetaCycle_Padova.Models.LTWorks;

namespace BetaCycle_Padova.Controllers.LTWorks
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerAddressesController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        public CustomerAddressesController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        // GET: api/CustomerAddresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerAddress>>> GetCustomerAddresses()
        {
            return await _context.CustomerAddresses.ToListAsync();
        }

        // GET: api/CustomerAddresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerAddress>> GetCustomerAddress(int id)
        {
            var customerAddress = await _context.CustomerAddresses.FindAsync(id);

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
            var customerAddress = await _context.CustomerAddresses.FirstOrDefaultAsync(c => c.CustomerId == custId);

            if (customerAddress == null)
            {
                return NotFound();
            }

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
            var customerAddress = await _context.CustomerAddresses.FirstOrDefaultAsync(a => a.AddressId == adrsId);

            if (customerAddress == null)
            {
                return NotFound();
            }

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
                return BadRequest();
            }

            _context.Entry(customerAddress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
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

            // Trova l'entità esistente nel database
            var customerAddress = await _context.CustomerAddresses
                                                .FirstOrDefaultAsync(ca => ca.CustomerId == custId);

            if (customerAddress == null)
            {
                return NotFound();
            }

            // Aggiorna solo i campi specifici
            //customerAddress.AddressId = customerAddressFE.AddressId;
            customerAddress.AddressType = customerAddressFE.AddressType;
            customerAddress.ModifiedDate = DateTime.Now;

            try
            {
                // Salva i cambiamenti nel database
                await _context.SaveChangesAsync();
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
            _context.CustomerAddresses.Add(customerAddress);
            try
            {
                await _context.SaveChangesAsync();
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
            CustomerAddress customerAddress = new CustomerAddress
            {
                AddressId = customerAddressFE.AddressId,
                CustomerId = customerAddressFE.CustomerId,
                AddressType = customerAddressFE.AddressType
            };

            _context.CustomerAddresses.Add(customerAddress);
            try
            {
                await _context.SaveChangesAsync();
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
            var customerAddress = await _context.CustomerAddresses.FindAsync(id);
            if (customerAddress == null)
            {
                return NotFound();
            }

            _context.CustomerAddresses.Remove(customerAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerAddressExists(int id)
        {
            return _context.CustomerAddresses.Any(e => e.CustomerId == id);
        }
    }
}
