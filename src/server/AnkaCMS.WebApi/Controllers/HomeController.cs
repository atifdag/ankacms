using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;
using AnkaCMS.Core;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]

    public class HomeController : ControllerBase
    {

        private readonly ICacheService _cacheService;

        public HomeController(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            return Dictionary.Success;
        }

        [Route("GlobalizationKeys")]
        [HttpGet]
        public ActionResult<List<string>> GlobalizationKeys()
        {




            var model= new List<string>();
            var cacheKey = "AnkaCMS.WebApi.Controllers.HomeController.GlobalizationKeys";
            if (_cacheService.Exists(cacheKey))
            {
                model = _cacheService.Get<List<string>>(cacheKey);
            }
            else
            {
                var resourceManagerDictionary = new ResourceManager(typeof(Dictionary));
                var resourceSetDictionary = resourceManagerDictionary.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                foreach (DictionaryEntry entry in resourceSetDictionary)
                {
                    model.Add("glb-dict-" + entry.Key);
                }
                var resourceManagerMessages = new ResourceManager(typeof(Messages));
                var resourceSetMessages = resourceManagerMessages.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                foreach (DictionaryEntry entry in resourceSetMessages)
                {
                    model.Add("glb-msg-" + entry.Key);
                }
                _cacheService.Add(cacheKey, model);
                _cacheService.AddToKeyList(cacheKey);

            }
            return Ok(model);
        }

    }
}
