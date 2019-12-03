using AnkaCMS.Service;
using AnkaCMS.Service.Models;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.Enums;
using AnkaCMS.Core.Exceptions;
using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ParameterController : ControllerBase
    {
        private readonly IMainService _serviceMain;
        private readonly IParameterService _serviceParameter;

        public ParameterController(IParameterService serviceParameter, IMainService serviceMain)
        {
            _serviceParameter = serviceParameter;
            _serviceMain = serviceMain;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<ParameterModel>> List()
        {
            ListModel<ParameterModel> model;

            try
            {
                var filterModel = new FilterModel
                {
                    StartDate = DateTime.Now.AddYears(-2),
                    EndDate = DateTime.Now,
                    Status = StatusOption.All.GetHashCode(),
                    PageNumber = 1,
                    PageSize = _serviceMain.ApplicationSettings.DefaultPageSize,
                    Searched = string.Empty
                };
                model = _serviceParameter.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<ParameterModel>
                {
                    HasError = true,
                    Message = exception.Message
                };
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return model;
            }
        }

        [Route("Filter")]
        [HttpPost]

        public ActionResult<ListModel<ParameterModel>> Filter(FilterModelWithParent filterModel)
        {
            try
            {
                return _serviceParameter.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<ParameterModel>> Detail(Guid id)
        {
            try
            {
                return Ok(_serviceParameter.Detail(id));
            }

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound + " " + exception);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpGet]

        public ActionResult<AddModel<ParameterModel>> Add()
        {
            try
            {
                return Ok(_serviceParameter.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<ParameterModel>> Add(AddModel<ParameterModel> addModel)
        {
            try
            {
                return Ok(_serviceParameter.Add(addModel));
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

        [Route("Update")]
        [HttpGet]

        public ActionResult<UpdateModel<ParameterModel>> Update(Guid id)
        {
            try
            {
                return Ok(_serviceParameter.Update(id));
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

        [Route("Update")]
        [HttpPut]
        public ActionResult<UpdateModel<ParameterModel>> Update(UpdateModel<ParameterModel> updateModel)
        {
            try
            {
                return Ok(_serviceParameter.Update(updateModel));
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

            catch (NotFoundException)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public ActionResult Delete(Guid id)
        {
            try
            {
                _serviceParameter.Delete(id);
                return Ok();
            }

            catch (InvalidTransactionException exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }

            catch (Exception)
            {
                ModelState.AddModelError("ErrorMessage", Messages.DangerRecordNotFound);
                return BadRequest(ModelState);
            }
        }
    }
}
