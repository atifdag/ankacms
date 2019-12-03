using AnkaCMS.Service;
using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GlobalizationDictionaryController : ControllerBase
    {

        private readonly IMainService _serviceMain;

        public GlobalizationDictionaryController(IMainService serviceMain)
        {
            _serviceMain = serviceMain;
        }

        [Route("Get")]
        [HttpGet]
        public ActionResult<string> Get([FromQuery] string key)
        {
            return key == "ApplicationName" ? _serviceMain.ApplicationSettings.ApplicationName : Dictionary.ResourceManager.GetString(key);
        }


    }
}