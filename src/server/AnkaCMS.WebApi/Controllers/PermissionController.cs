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
    public class PermissionController : ControllerBase, ICrudController<PermissionModel>
    {
        private readonly IMainService _serviceMain;
        private readonly IPermissionService _servicePermission;

        public PermissionController(IPermissionService servicePermission, IMainService serviceMain)
        {
            _servicePermission = servicePermission;
            _serviceMain = serviceMain;
        }

        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<PermissionModel>> List()
        {
            ListModel<PermissionModel> model;

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
                model = _servicePermission.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<PermissionModel>
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

        public ActionResult<ListModel<PermissionModel>> Filter(FilterModel filterModel)
        {
            try
            {
                return _servicePermission.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<PermissionModel>> Detail(Guid id)
        {
            try
            {
                return Ok(_servicePermission.Detail(id));
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

        public ActionResult<AddModel<PermissionModel>> Add()
        {
            try
            {
                return Ok(_servicePermission.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<PermissionModel>> Add(AddModel<PermissionModel> addModel)
        {
            try
            {
                return Ok(_servicePermission.Add(addModel));
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

        public ActionResult<UpdateModel<PermissionModel>> Update(Guid id)
        {
            try
            {
                return Ok(_servicePermission.Update(id));
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
        public ActionResult<UpdateModel<PermissionModel>> Update(UpdateModel<PermissionModel> updateModel)
        {
            try
            {
                return Ok(_servicePermission.Update(updateModel));
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
                _servicePermission.Delete(id);
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
