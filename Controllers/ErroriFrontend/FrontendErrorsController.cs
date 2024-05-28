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
    [Route("api/[controller]")]
    [ApiController]
    public class FrontendErrorsController : ControllerBase
    {
        private readonly AdventureWorksLt2019Context _context;

        private static Logger FrontendErrorsNlogLogger = LogManager.GetCurrentClassLogger();

        public FrontendErrorsController(AdventureWorksLt2019Context context)
        {
            _context = context;
        }

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
                FrontendErrorsNlogLogger.Error(dbEx, "PostFrontendError ha sollevato un'eccezione 'DbUpdate' nel catch");
                return BadRequest(new { message = "DbUpdate Exception - see backend" });
            }
            catch (Exception ex)
            {
                FrontendErrorsNlogLogger.Error(ex, "PostFrontendError ha sollevato un'eccezione generica nel catch");
                return BadRequest(new { message = "generic Exception - see backend" });
            }

            return Ok(new { message = "Error successfully posted to DB tab LogTrace" });
        }

        
    }
}
