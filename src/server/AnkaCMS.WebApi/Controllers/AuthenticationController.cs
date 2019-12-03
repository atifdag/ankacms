using AnkaCMS.Service;
using AnkaCMS.Service.Models;
using AnkaCMS.Core.Enums;
using AnkaCMS.Core.Exceptions;
using AnkaCMS.Core.Globalization;
using AnkaCMS.Core.Helpers;
using AnkaCMS.Core.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IAuthenticationService _serviceAuthentication;
        private readonly IIdentityService _serviceIdentity;
        private readonly IConfiguration _configuration;

        public AuthenticationController(IAuthenticationService serviceAuthentication, IIdentityService serviceIdentity, IConfiguration configuration)
        {
            _serviceAuthentication = serviceAuthentication;
            _serviceIdentity = serviceIdentity;
            _configuration = configuration;
        }

        [Route("SignIn")]
        [HttpPost]
        public ActionResult<string> SignIn([FromBody] SignInModel model)
        {

            try
            {
                if (model.Key == _configuration.GetSection("JwtSecurityKey").Value)
                {
                    _serviceAuthentication.SignIn(model);
                    var principal = (AnkaCMSPrincipal)Thread.CurrentPrincipal;
                    var identity = (AnkaCMSIdentity)Thread.CurrentPrincipal.Identity;

                    _serviceIdentity.Set(identity, DateTime.Now.AddMinutes(20), false);
                    HttpContext.User = principal;
                    var jwt = SecurityHelper.GetJwt(identity, _configuration.GetSection("JwtSecurityKey").Value);
                    return jwt;
                }
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Messages.DangerInvalidApiClient;

            }

            catch (Exception exception)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return exception.Message;
            }
        }

        [Route("SignUp")]
        [HttpPost]
        public ActionResult SignUp([FromBody] SignUpModel signUpModel)
        {
            try
            {
                if (signUpModel.IdentityCode == null)
                {
                    signUpModel.IdentityCode = GuidHelper.NewGuid().ToString();
                }
                _serviceAuthentication.SignUp(signUpModel);
                return Ok();
            }

            catch (ValidationException exception)
            {
                var validationResult = exception.ValidationResult;
                foreach (var t in validationResult)
                {
                    ModelState.AddModelError(t.PropertyName, t.ErrorMessage);
                }
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }


        [Route("SignOut")]
        [HttpGet]
        public void SignOut(SignOutOption signOutOption = SignOutOption.ValidLogout)
        {
            _serviceAuthentication.SignOut(signOutOption);
            var principal = (AnkaCMSPrincipal)Thread.CurrentPrincipal;
            HttpContext.User = principal;
        }


        [Route("ForgotPassword")]
        [HttpGet]
        public ActionResult<string> ForgotPassword([FromQuery] string username)
        {
            try
            {
                _serviceAuthentication.ForgotPassword(username);
                return Messages.InfoPasswordSentSuccesfully;
            }

            catch (Exception exception)
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return exception.Message;
            }
        }


    }
}