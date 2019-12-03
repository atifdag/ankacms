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
using AnkaCMS.Core;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class PartController : ControllerBase
    {
        private readonly IMainService _serviceMain;
        private readonly IPartService _servicePart;
        private readonly ICacheService _cacheService;

        public PartController(IPartService servicePart, IMainService serviceMain, ICacheService cacheService)
        {
            _servicePart = servicePart;
            _serviceMain = serviceMain;
            _cacheService = cacheService;
        }


        [Route("GetPublicCarouselContents")]
        [HttpGet]
        public ActionResult<PublicPartModel> GetPublicCarouselContents(string partCode, string languageCode)
        {
            try
            {
                PublicPartModel model;
                var cacheKey = "AnkaCMS.WebApi.Controllers.PartController.GetPublicCarouselContents-" + partCode+"-"+ languageCode;
                if (_cacheService.Exists(cacheKey))
                {
                    model = _cacheService.Get<PublicPartModel>(cacheKey);
                }
                else
                {
                    model = _servicePart.GetPublicCarouselContents(partCode, languageCode);
                    _cacheService.Add(cacheKey, model);
                    _cacheService.AddToKeyList(cacheKey);

                }
                return Ok(model);



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




        [Route("List")]
        [HttpGet]
        public ActionResult<ListModel<PartModel>> List()
        {
            ListModel<PartModel> model;

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
                model = _servicePart.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<PartModel>
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

        public ActionResult<ListModel<PartModel>> Filter(FilterModelWithLanguage filterModel)
        {
            try
            {
                return _servicePart.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<PartModel>> Detail(Guid partId, Guid languageId)
        {
            try
            {
                return Ok(_servicePart.Detail(partId, languageId));
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

        public ActionResult<AddModel<PartModel>> Add()
        {
            try
            {
                return Ok(_servicePart.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<PartModel>> Add(AddModel<PartModel> addModel)
        {
            try
            {
                return Ok(_servicePart.Add(addModel));
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

        public ActionResult<UpdateModel<PartModel>> Update(Guid partId, Guid languageId)
        {
            try
            {
                return Ok(_servicePart.Update(partId, languageId));
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
        public ActionResult<UpdateModel<PartModel>> Update(UpdateModel<PartModel> updateModel)
        {
            try
            {
                return Ok(_servicePart.Update(updateModel));
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
        public ActionResult Delete(Guid partId, Guid languageId)
        {
            try
            {
                _servicePart.Delete(partId, languageId);
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
        public ActionResult<List<KeyValuePair<Guid, string>>> KeysAndValues(Guid languageId)
        {
            try
            {
                return Ok(_servicePart.List(languageId));
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
