using AnkaCMS.Service;
using AnkaCMS.Service.Models;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.Enums;
using AnkaCMS.Core.Exceptions;
using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ParameterGroupController : ControllerBase, ICrudController<ParameterGroupModel>
    {
        private readonly IMainService _serviceMain;
        private readonly IParameterGroupService _serviceParameterGroup;

        public ParameterGroupController(IParameterGroupService serviceParameterGroup, IMainService serviceMain)
        {
            _serviceParameterGroup = serviceParameterGroup;
            _serviceMain = serviceMain;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<ParameterGroupModel>> List()
        {
            ListModel<ParameterGroupModel> model;

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
                model = _serviceParameterGroup.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<ParameterGroupModel>
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

        public ActionResult<ListModel<ParameterGroupModel>> Filter(FilterModel filterModel)
        {
            try
            {
                return _serviceParameterGroup.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<ParameterGroupModel>> Detail(Guid id)
        {
            try
            {
                return Ok(_serviceParameterGroup.Detail(id));
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

        public ActionResult<AddModel<ParameterGroupModel>> Add()
        {
            try
            {
                return Ok(_serviceParameterGroup.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<ParameterGroupModel>> Add(AddModel<ParameterGroupModel> addModel)
        {
            try
            {
                return Ok(_serviceParameterGroup.Add(addModel));
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

        public ActionResult<UpdateModel<ParameterGroupModel>> Update(Guid id)
        {
            try
            {
                return Ok(_serviceParameterGroup.Update(id));
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
        public ActionResult<UpdateModel<ParameterGroupModel>> Update(UpdateModel<ParameterGroupModel> updateModel)
        {
            try
            {
                return Ok(_serviceParameterGroup.Update(updateModel));
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
                _serviceParameterGroup.Delete(id);
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

        [Route("KeysAndValues")]
        [HttpGet]
        public ActionResult<List<KeyValuePair<Guid, string>>> KeysAndValues()
        {
            try
            {
                return Ok(_serviceParameterGroup.List());
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

    }
}
