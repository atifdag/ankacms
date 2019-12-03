using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using AnkaCMS.Core;
using AnkaCMS.Core.Globalization;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]

    public class CacheController : ControllerBase
    {

        private readonly ICacheService _cacheService;

        public CacheController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<List<string>> List()
        {
            var model = _cacheService.GetAllKeyList();
            return Ok(model);
        }


        [Route("Delete")]
        [HttpDelete]
        public ActionResult Delete(string key)
        {
            try
            {
                _cacheService.Remove(key);
                _cacheService.RemoveFromKeyList(key);
                return Ok();
            }

           
            catch (Exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }
        }

    }
}
