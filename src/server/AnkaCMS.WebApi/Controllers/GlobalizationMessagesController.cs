using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class GlobalizationMessagesController : ControllerBase
    {
        [Route("Get")]
        [HttpGet]
        public ActionResult<string> Get([FromQuery] string key)
        {
            return Messages.ResourceManager.GetString(key);
        }

        [Route("GetByParameter")]
        [HttpGet]
        public ActionResult<string> GetByParameter([FromQuery] string key, string parameter)
        {
            return string.Format(Messages.ResourceManager.GetString(key),
                Dictionary.ResourceManager.GetString(parameter));
            //string.Format(Messages.DangerFieldLengthLimit, Dictionary.Username, "8")
        }

        [Route("GetByParameter2")]
        [HttpGet]
        public ActionResult<string> GetByParameter2([FromQuery] string key, string parameter1, string parameter2)
        {
            return string.Format(Messages.ResourceManager.GetString(key), Dictionary.ResourceManager.GetString(parameter1), parameter2);
        }

    }
}