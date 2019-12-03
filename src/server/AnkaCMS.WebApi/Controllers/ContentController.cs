using AnkaCMS.Service;
using AnkaCMS.Service.Models;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.Enums;
using AnkaCMS.Core.Exceptions;
using AnkaCMS.Core.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using AnkaCMS.Core;

namespace AnkaCMS.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ContentController : ControllerBase
    {
        private readonly IMainService _serviceMain;
        private readonly IContentService _serviceContent;
        private readonly ICacheService _cacheService;

        public ContentController(IContentService serviceContent, IMainService serviceMain, ICacheService cacheService)
        {
            _serviceContent = serviceContent;
            _serviceMain = serviceMain;
            _cacheService = cacheService;
        }


        [Route("PublicDetail")]
        [HttpGet]
        public ActionResult<PublicContentModel> PublicDetail(string code)
        {
            try
            {
                PublicContentModel model;
                var cacheKey = "AnkaCMS.WebApi.Controllers.ContentController.PublicDetail-" + code;
                if (_cacheService.Exists(cacheKey))
                {
                    model = _cacheService.Get<PublicContentModel>(cacheKey);
                }
                else
                {
                    model = _serviceContent.PublicDetail(code);
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
        public ActionResult<ListModel<ContentModel>> List()
        {
            ListModel<ContentModel> model;

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
                model = _serviceContent.List(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<ContentModel>
                {
                    HasError = true,
                    Message = exception.Message
                };
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return model;
            }
        }

        [Route("MyContentList")]
        [HttpGet]
        public ActionResult<ListModel<ContentModel>> MyContentList()
        {
            ListModel<ContentModel> model;

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
                model = _serviceContent.MyContentList(filterModel);
                return Ok(model);
            }

            catch (Exception exception)
            {
                model = new ListModel<ContentModel>
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

        public ActionResult<ListModel<ContentModel>> Filter(FilterModelWithLanguageAndParent filterModel)
        {
            try
            {
                return _serviceContent.List(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("MyContentFilter")]
        [HttpPost]

        public ActionResult<ListModel<ContentModel>> MyContentFilter(FilterModelWithLanguageAndParent filterModel)
        {
            try
            {
                return _serviceContent.MyContentList(filterModel);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }


        [Route("Detail")]
        [HttpGet]

        public ActionResult<DetailModel<ContentModel>> Detail(Guid contentId, Guid languageId)
        {
            try
            {
                return Ok(_serviceContent.Detail(contentId, languageId));
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

        [Route("MyContentDetail")]
        [HttpGet]

        public ActionResult<DetailModel<ContentModel>> MyContentDetail(Guid contentId, Guid languageId)
        {
            try
            {
                return Ok(_serviceContent.MyContentDetail(contentId, languageId));
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

        public ActionResult<AddModel<ContentModel>> Add()
        {
            try
            {
                return Ok(_serviceContent.Add());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("MyContentAdd")]
        [HttpGet]

        public ActionResult<AddModel<ContentModel>> MyContentAdd()
        {
            try
            {
                return Ok(_serviceContent.MyContentAdd());
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }


        [Route("Add")]
        [HttpPost]
        public ActionResult<AddModel<ContentModel>> Add(AddModel<ContentModel> addModel)
        {
            try
            {
                return Ok(_serviceContent.Add(addModel));
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


        [Route("MyContentAdd")]
        [HttpPost]
        public ActionResult<AddModel<ContentModel>> MyContentAdd(AddModel<ContentModel> addModel)
        {
            try
            {
                return Ok(_serviceContent.MyContentAdd(addModel));
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

        public ActionResult<UpdateModel<ContentModel>> Update(Guid contentId, Guid languageId)
        {
            try
            {
                return Ok(_serviceContent.Update(contentId, languageId));
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


        [Route("MyContentUpdate")]
        [HttpGet]

        public ActionResult<UpdateModel<ContentModel>> MyContentUpdate(Guid contentId, Guid languageId)
        {
            try
            {
                return Ok(_serviceContent.MyContentUpdate(contentId, languageId));
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
        public ActionResult<UpdateModel<ContentModel>> Update(UpdateModel<ContentModel> updateModel)
        {
            try
            {
                return Ok(_serviceContent.Update(updateModel));
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

            catch (NotFoundException ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.ToString());
                return BadRequest(ModelState);
            }

            catch (Exception exception)
            {
                ModelState.AddModelError("ErrorMessage", exception.Message);
                return BadRequest(ModelState);
            }
        }

        [Route("MyContentUpdate")]
        [HttpPut]
        public ActionResult<UpdateModel<ContentModel>> MyContentUpdate(UpdateModel<ContentModel> updateModel)
        {
            try
            {
                return Ok(_serviceContent.MyContentUpdate(updateModel));
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

            catch (NotFoundException ex)
            {
                ModelState.AddModelError("ErrorMessage", ex.ToString());
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
        public ActionResult Delete(Guid contentId, Guid languageId)
        {
            try
            {
                _serviceContent.Delete(contentId, languageId);
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

        [Route("MyContentDelete")]
        [HttpDelete]
        public ActionResult MyContentDelete(Guid contentId, Guid languageId)
        {
            try
            {
                _serviceContent.MyContentDelete(contentId, languageId);
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
