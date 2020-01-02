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
    public class CategoryController : ControllerBase
    {
        private readonly IMainService _serviceMain;
        private readonly ICategoryService _serviceCategory;
        private readonly ICacheService _cacheService;

        public CategoryController(ICategoryService serviceCategory, IMainService serviceMain, ICacheService cacheService)
        {
            _serviceCategory = serviceCategory;
            _serviceMain = serviceMain;
            _cacheService = cacheService;
        }



        [Route("PublicDetail")]
        [HttpGet]
        public ActionResult<PublicCategoryModel> PublicDetail(string code)
        {
            try
            {
                PublicCategoryModel model;
                var cacheKey = "AnkaCMS.WebApi.Controllers.CategoryController.PublicDetail-" + code;
                if (_cacheService.Exists(cacheKey))
                {
                    model = _cacheService.Get<PublicCategoryModel>(cacheKey);
                }
                else
                {
                    model = _serviceCategory.PublicDetail(code);
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
        public ActionResult<ListModel<CategoryModel>> List()
        {
            ListModel<CategoryModel> model;

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
                model = _serviceCategory.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<CategoryModel>
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

        public ActionResult<ListModel<CategoryModel>> Filter(FilterModelWithLanguage filterModel)
        {
            try
            {
                return _serviceCategory.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<CategoryModel>> Detail(Guid categoryId, Guid languageId)
        {
            try
            {
                return Ok(_serviceCategory.Detail(categoryId, languageId));
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

        public ActionResult<AddModel<CategoryModel>> Add()
        {
            try
            {
                return Ok(_serviceCategory.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<CategoryModel>> Add(AddModel<CategoryModel> addModel)
        {
            try
            {
                return Ok(_serviceCategory.Add(addModel));
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

        public ActionResult<UpdateModel<CategoryModel>> Update(Guid categoryId, Guid languageId)
        {
            try
            {
                return Ok(_serviceCategory.Update(categoryId, languageId));
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
        public ActionResult<UpdateModel<CategoryModel>> Update(UpdateModel<CategoryModel> updateModel)
        {
            try
            {
                return Ok(_serviceCategory.Update(updateModel));
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
        public ActionResult Delete(Guid categoryId, Guid languageId)
        {
            try
            {
                _serviceCategory.Delete(categoryId, languageId);
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
                return Ok(_serviceCategory.List());
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
