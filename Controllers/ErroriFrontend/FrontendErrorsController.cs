using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BetaCycle_Padova.Controllers.Context;
using Microsoft.Data.SqlClient;
using DnsClient;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.CompilerServices;
using BetaCycle_Padova.Models.LTWorks;
using BetaCycle_Padova.Models.LTWorks.Views;
using NLog;
using NLog.Fluent;

namespace BetaCycle_Padova.Controllers.ErroriFrontend
{
    /// <summary>
    /// API Controller for handling frontend errors.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendErrorsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;
        private static Logger FrontendErrorsNlogLogger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="FrontendErrorsController"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public FrontendErrorsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

        /// <summary>
        /// Logs a frontend error to the database.
        /// </summary>
        /// <param name="fEndError">The frontend error log trace.</param>
        /// <returns>An action result indicating the outcome of the operation.</returns>
        [HttpPost]
        public async Task<ActionResult<LogTrace>> PostFrontendError(LogTrace fEndError)
        {
            _context.LogTraces.Add(fEndError);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                FrontendErrorsNlogLogger.Error(dbEx, "PostFrontendError raised a 'DbUpdate' exception in the catch block");

                return BadRequest(new { message = "DbUpdate Exception - see backend" });
            }
            catch (Exception ex)
            {
                FrontendErrorsNlogLogger.Error(ex, "PostFrontendError raised a generic exception in the catch block");
                return BadRequest(new { message = "Generic Exception - see backend" });
            }

            return Ok(new { message = "Error successfully posted to DB tab LogTrace" });
        }
    }
}
