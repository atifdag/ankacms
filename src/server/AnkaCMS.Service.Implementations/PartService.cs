using AnkaCMS.Data.DataAccess.EntityFramework;
using AnkaCMS.Data.DataEntities;
using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.Exceptions;
using AnkaCMS.Core.Globalization;
using AnkaCMS.Core.Helpers;
using AnkaCMS.Core.Security;
using AnkaCMS.Core.Validation.FluentValidation;
using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AnkaCMS.Service.Implementations.ValidationRules.FluentValidation;

namespace AnkaCMS.Service.Implementations
{
    public class PartService : IPartService
    {

        private readonly IRepository<Part> _repositoryPart;
        private readonly IRepository<PartHistory> _repositoryPartHistory;
        private readonly IRepository<PartLanguageLine> _repositoryPartLanguageLine;
        private readonly IRepository<PartLanguageLineHistory> _repositoryPartLanguageLineHistory;
        private readonly IRepository<Language> _repositoryLanguage;
        private readonly IRepository<PartContentLine> _repositoryPartContentLine;
        private readonly IRepository<PartContentLineHistory> _repositoryPartContentLineHistory;
        private readonly IRepository<Content> _repositoryContent;
        private readonly IRepository<User> _repositoryUser;

        private readonly Language _defaultLanguage;
        private readonly IMainService _serviceMain;
        public PartService(IRepository<Part> repositoryPart, IRepository<PartHistory> repositoryPartHistory, IRepository<Language> repositoryLanguage, IRepository<User> repositoryUser, IRepository<PartLanguageLine> repositoryPartLanguageLine, IRepository<PartLanguageLineHistory> repositoryPartLanguageLineHistory, IRepository<PartContentLine> repositoryPartContentLine, IRepository<Content> repositoryContent, IRepository<PartContentLineHistory> repositoryPartContentLineHistory, IMainService serviceMain)
        {
            _repositoryPart = repositoryPart;
            _repositoryPartHistory = repositoryPartHistory;
            _repositoryLanguage = repositoryLanguage;
            _repositoryUser = repositoryUser;

            _repositoryPartLanguageLine = repositoryPartLanguageLine;
            _repositoryPartLanguageLineHistory = repositoryPartLanguageLineHistory;
            _repositoryPartContentLine = repositoryPartContentLine;
            _repositoryContent = repositoryContent;
            _repositoryPartContentLineHistory = repositoryPartContentLineHistory;
            _serviceMain = serviceMain;
            _defaultLanguage = _repositoryLanguage.Get(x => x.DisplayOrder == 1);
        }


        private User IdentityUser
        {
            get
            {
                // Thread'de kayıtlı kimlik bilgisi alınıyor
                var identity = (AnkaCMSIdentity)Thread.CurrentPrincipal.Identity;

                User user;

                // Veritabanından sorgulanıyor
                user = _repositoryUser.Get()
                    .Join(x => x.Person)
                    .FirstOrDefault(a => a.Id == identity.UserId);




                // Kullanıcı bulunamadı ise
                if (user == null)
                {
                    throw new NotFoundException(Messages.DangerIdentityUserNotFound);
                }

                return user;
            }
        }

        public ListModel<PartModel> List(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<PartModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, Guid.Empty, model);
        }

        public ListModel<PartModel> List(FilterModelWithLanguage filterModel)
        {
            var model = filterModel.CreateMapped<FilterModelWithLanguage, ListModel<PartModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Language.Id, model);
        }

        public DetailModel<PartModel> Detail(Guid id)
        {
            return Detail(id, _defaultLanguage.Id);
        }


        public DetailModel<PartModel> Detail(Guid partId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var itemPart = _repositoryPart
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.PartLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == partId);

            if (itemPart == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var allContents = _repositoryContent.Get().Select(x=> new IdCodeName(x.Id, x.Code, x.Code)).ToList();

            var itemPartContents = _repositoryPartContentLine
                .Join(x => x.Content)
                .ThenJoin(x => x.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .OrderBy(x=>x.DisplayOrder)
                .Where(x => x.Part.Id == itemPart.Id).Select(x => x.Content.Id).ToList();


            var unSelectedContents = new List<IdCodeNameSelected>();
            var selectedContents = new List<IdCodeNameSelected>();

            foreach (var itemContent in allContents)
            {
                if (itemPartContents.Contains(itemContent.Id))
                {
                    selectedContents.Add(new IdCodeNameSelected(itemContent.Id, itemContent.Code, itemContent.Code, true));
                }
                else
                {
                    unSelectedContents.Add(new IdCodeNameSelected(itemContent.Id, itemContent.Code, itemContent.Code, false));
                }
            }

            var orderedSelectedContents = itemPartContents.Select(itemContent => selectedContents.FirstOrDefault(x => x.Id == itemContent)).ToList();
            
            PartModel modelItem;
            if (itemPart.PartLanguageLines == null)
            {
                modelItem = new PartModel();
            }
            else
            {
                var itemLine = itemPart.PartLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                modelItem = itemLine != null ? itemLine.CreateMapped<PartLanguageLine, PartModel>() : new PartModel();
            }

            modelItem.Creator = new IdCodeName(itemPart.Creator.Id, itemPart.Creator.Username, itemPart.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(itemPart.LastModifier.Id, itemPart.LastModifier.Username, itemPart.LastModifier.Person.DisplayName);
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.PartId = itemPart.Id;


            var modelItemContents = unSelectedContents;

            modelItemContents.AddRange(orderedSelectedContents);

            modelItem.Contents = modelItemContents;

            return new DetailModel<PartModel>
            {
                Item = modelItem
            };
        }

        public List<IdCodeName> List(Guid languageId)
        {
            var list = _repositoryPartLanguageLine.Get().Where(x => x.Language.Id == languageId).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id,x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }
            throw new NotFoundException(Messages.DangerRecordNotFound);
        }

        public PublicPartModel GetPublicPartContents(string partCode, string languageCode)
        {
            var language = _repositoryLanguage.Get(x => x.Code == languageCode);

            var item = _repositoryPart
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x=>x.PartContentLines)
                .ThenJoin(x=>x.Content)
                .Join(z => z.PartLanguageLines)
                
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Code == partCode);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (item.PartLanguageLines == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var listContent = new List<PublicContentModel>();



            var itemIds = item.PartContentLines.OrderBy(t=>t.DisplayOrder).Select(t => t.Content).Select(c => c.Id).ToList();

         //   var allContent = _repositoryContent.Get();

         var itemContents = _repositoryContent
             .Join(x => x.Category)
             .ThenJoin(x => x.CategoryLanguageLines)
             .ThenJoin(x => x.Language)
             .Join(x => x.ContentLanguageLines)
             .ThenJoin(x => x.Language)
//             .OrderBy(x=>x.)
             .Where(z => itemIds.Contains(z.Id)).ToList();
            foreach (var itemContent in itemContents)
            {
                var contentLanguageLine = itemContent.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                if (contentLanguageLine != null)
                {
                    var publicContentModel = contentLanguageLine.CreateMapped<ContentLanguageLine, PublicContentModel>();
                    publicContentModel.ContentId = itemContent.Id;

                    var category = itemContent.Category.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    if (category != null)
                    {
                        publicContentModel.Category = new IdCodeName(itemContent.Category.Id, itemContent.Category.Code, category.Name);
                    }
                    publicContentModel.Language = new IdCodeName(language.Id, language.Code, language.Name);

                    listContent.Add(publicContentModel);
                }
            }



            var itemLine = item.PartLanguageLines.FirstOrDefault(x => x.Language.Code == languageCode);
            var model = itemLine.CreateMapped<PartLanguageLine, PublicPartModel>();
            model.Language = new IdCodeName(language.Id, language.Code, language.Name);
            model.PartId = item.Id;
            model.Contents = listContent;

            return model;


        }

        private ListModel<PartModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid languageId, ListModel<PartModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();
            var language = languageId != Guid.Empty ? _repositoryLanguage.Get(x => x.Id == languageId) : _defaultLanguage;

            Expression<Func<Part, bool>> expression;

            if (model.Paging == null)
            {
                model.Paging = new Paging
                {
                    PageSize = pageSize,
                    PageNumber = pageNumber
                };
            }

            if (status != -1)
            {
                var bStatus = status.ToString().ToBoolean();
                if (searched != null)
                {
                    if (bStatus)
                    {
                        expression = c => c.PartLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.PartLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved == false);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.PartLanguageLines.Any(x => x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.PartLanguageLines.Any(x => x.IsApproved == false);
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.PartLanguageLines.Any(x => x.Name.Contains(searched));
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            var sortHelper = new SortHelper<PartModel>();

            var query = _repositoryPart
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.PartLanguageLines)
                .ThenJoin(x => x.Language)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<PartModel>();
            foreach (var item in items)
            {
                PartModel modelItem;
                if (item.PartLanguageLines == null)
                {
                    modelItem = new PartModel();
                }
                else
                {
                    var itemLine = item.PartLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    modelItem = itemLine != null ? itemLine.CreateMapped<PartLanguageLine, PartModel>() : new PartModel();
                }

                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
                modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
                modelItem.PartId = item.Id;
                modelItems.Add(modelItem);
            }
            model.Items = modelItems.ToList();
            var pageSizeDescription = _serviceMain.ApplicationSettings.PageSizeList;
            var pageSizes = pageSizeDescription.Split(',').Select(s => new KeyValuePair<int, string>(s.ToInt(), s)).ToList();
            pageSizes.Insert(0, new KeyValuePair<int, string>(-1, "[" + Dictionary.All + "]"));
            model.Paging.PageSizes = pageSizes;
            model.Paging.PageCount = (int)Math.Ceiling((float)model.Paging.TotalItemCount / model.Paging.PageSize);
            if (model.Paging.TotalItemCount > model.Items.Count)
            {
                model.Paging.HasNextPage = true;
            }

            // ilk sayfa ise

            if (model.Paging.PageNumber == 1)
            {
                if (model.Paging.TotalItemCount > 0)
                {
                    model.Paging.IsFirstPage = true;
                }

                // tek sayfa ise

                if (model.Paging.PageCount == 1)
                {
                    model.Paging.IsLastPage = true;
                }

            }

            // son sayfa ise
            else if (model.Paging.PageNumber == model.Paging.PageCount)
            {
                model.Paging.HasNextPage = false;
                // tek sayfa değilse

                if (model.Paging.PageCount > 1)
                {
                    model.Paging.IsLastPage = true;
                    model.Paging.HasPreviousPage = true;
                }
            }

            // ara sayfa ise
            else
            {
                model.Paging.HasNextPage = true;
                model.Paging.HasPreviousPage = true;
            }

            if (model.Paging.TotalItemCount > model.Items.Count && model.Items.Count <= 0)
            {
                model.Message = Messages.DangerRecordNotFoundInPage;
            }

            if (model.Paging.TotalItemCount == 0)
            {
                model.Message = Messages.DangerRecordNotFound;
            }
            return model;
        }

        public AddModel<PartModel> Add()
        {
            return new AddModel<PartModel>
            {
                Item = new PartModel
                {
                    IsApproved = false
                }
            };
        }

        public AddModel<PartModel> Add(AddModel<PartModel> addModel)
        {
            IValidator validator = new FluentValidator<PartModel, PartValidationRules>(addModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var language = _repositoryLanguage.Get(e => e.Id == addModel.Item.Language.Id);

            if (language == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            var line = addModel.Item.CreateMapped<PartModel, PartLanguageLine>();

            if (_repositoryPartLanguageLine.Get().FirstOrDefault(e => e.Code == line.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            var item = new Part
            {
                Code = GuidHelper.NewGuid().ToString(),
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                Creator = IdentityUser,
                LastModificationTime = DateTime.Now,
                LastModifier = IdentityUser
                
            };

            var affectedItem = _repositoryPart.Add(item, true);
            var itemHistory = affectedItem.CreateMapped<Part, PartHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryPartHistory.Add(itemHistory, true);

            var maxLineDisplayOrder = _repositoryPartLanguageLine.Get().Where(x => x.Language.Id == addModel.Item.Language.Id).Max(e => e.DisplayOrder);

            line.Id = GuidHelper.NewGuid();
            line.Version = 1;
            line.DisplayOrder = maxLineDisplayOrder + 1;
            line.CreationTime = DateTime.Now;
            line.Language = language;
            line.Part = affectedItem;
            line.LastModificationTime = DateTime.Now;
            line.Creator = IdentityUser;
            line.LastModifier = IdentityUser;
            var affectedLine = _repositoryPartLanguageLine.Add(line, true);

            var lineHistory = affectedLine.CreateMapped<PartLanguageLine, PartLanguageLineHistory>();
            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = affectedLine.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.PartId = affectedLine.Part.Id;
            lineHistory.LanguageId = affectedLine.Language.Id;

            _repositoryPartLanguageLineHistory.Add(lineHistory, true);

            addModel.Item = affectedItem.CreateMapped<Part, PartModel>();



            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdCodeName(language.Id,language.Code, language.Name);
            return addModel;
        }

        public UpdateModel<PartModel> Update(Guid id)
        {
            return Update(id, _defaultLanguage.Id);
        }

        public UpdateModel<PartModel> Update(Guid partId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var itemPart = _repositoryPart
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.PartLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == partId);

            if (itemPart == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }



            var allContents = _repositoryContent.Get().Select(x => new IdCodeName(x.Id, x.Code, x.Code)).ToList();

            var itemPartContents = _repositoryPartContentLine
                .Join(x => x.Content)
                .ThenJoin(x => x.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .OrderBy(x => x.DisplayOrder)
                .Where(x => x.Part.Id == itemPart.Id).Select(x => x.Content.Id).ToList();


            var unSelectedContents = new List<IdCodeNameSelected>();
            var selectedContents = new List<IdCodeNameSelected>();

            foreach (var itemContent in allContents)
            {
                if (itemPartContents.Contains(itemContent.Id))
                {
                    selectedContents.Add(new IdCodeNameSelected(itemContent.Id, itemContent.Code, itemContent.Code, true));
                }
                else
                {
                    unSelectedContents.Add(new IdCodeNameSelected(itemContent.Id, itemContent.Code, itemContent.Code, false));
                }
            }

            var orderedSelectedContents = itemPartContents.Select(itemContent => selectedContents.FirstOrDefault(x => x.Id == itemContent)).ToList();

            PartModel modelItem;
            if (itemPart.PartLanguageLines == null)
            {
                modelItem = new PartModel();
            }
            else
            {
                var itemLine = itemPart.PartLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                if (itemLine != null)
                {
                    modelItem = itemLine.CreateMapped<PartLanguageLine, PartModel>();
                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(itemLine.LastModifier.Id,itemLine.LastModifier.Username, itemLine.LastModifier.Person.DisplayName);
                }
                else
                {
                    modelItem = new PartModel();
                }
            }

            var modelItemContents = unSelectedContents;

            modelItemContents.AddRange(orderedSelectedContents);

            modelItem.Contents = modelItemContents;
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.PartId = itemPart.Id;


            return new UpdateModel<PartModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<PartModel> Update(UpdateModel<PartModel> updateModel)
        {
            IValidator validator = new FluentValidator<PartModel, PartValidationRules>(updateModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var language = _repositoryLanguage.Get(e => e.Id == updateModel.Item.Language.Id);

            if (language == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            var item = _repositoryPart
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.PartLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == updateModel.Item.PartId);



            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            PartModel modelItem;
            if (item.PartLanguageLines == null)
            {
                modelItem = new PartModel();
            }
            else
            {
                var itemLine = item.PartLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);

                // güncelleme yapılacak
                if (itemLine != null)
                {
                    var version = itemLine.Version;
                    var lineHistory = itemLine.CreateMapped<PartLanguageLine, PartLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = itemLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.PartId = item.Id;
                    lineHistory.LanguageId = language.Id;
                    _repositoryPartLanguageLineHistory.Add(lineHistory, true);

                    itemLine.Code = updateModel.Item.Code;
                    itemLine.Name = updateModel.Item.Name;
                    itemLine.Description = updateModel.Item.Description;
                    itemLine.Keywords = updateModel.Item.Keywords;
                    itemLine.Version = version + 1;

                    itemLine.IsApproved = updateModel.Item.IsApproved;
                    itemLine.Part = item;
                    itemLine.LastModifier = IdentityUser;
                    itemLine.LastModificationTime = DateTime.Now;
                    var affectedItemLine = _repositoryPartLanguageLine.Update(itemLine, true);
                    modelItem = affectedItemLine.CreateMapped<PartLanguageLine, PartModel>();


                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);

                }

                // ekleme yapılacak
                else
                {
                    modelItem = new PartModel();

                    var maxLineDisplayOrder = _repositoryPartLanguageLine.Get().Where(x => x.Language.Id == language.Id).Max(e => e.DisplayOrder);

                    var line = updateModel.Item.CreateMapped<PartModel, PartLanguageLine>();

                    line.Id = GuidHelper.NewGuid();
                    line.Version = 1;
                    line.DisplayOrder = maxLineDisplayOrder + 1;
                    line.CreationTime = DateTime.Now;
                    line.Language = language;
                    line.Part = item;
                    line.LastModificationTime = DateTime.Now;
                    line.Creator = IdentityUser;
                    line.LastModifier = IdentityUser;
                    var affectedLine = _repositoryPartLanguageLine.Add(line, true);

                    var lineHistory = affectedLine.CreateMapped<PartLanguageLine, PartLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = affectedLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.PartId = affectedLine.Part.Id;
                    lineHistory.LanguageId = affectedLine.Language.Id;

                    _repositoryPartLanguageLineHistory.Add(lineHistory, true);


                }
            }

           


            var itemHistory = item.CreateMapped<Part, PartHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreatorId = IdentityUser.Id;

            _repositoryPartHistory.Add(itemHistory, true);
            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            var affectedItem = _repositoryPart.Update(item, true);


            foreach (var line in _repositoryPartContentLine
                .Join(x => x.Part)
                .Join(x => x.Content)
                .Where(x => x.Part.Id == affectedItem.Id).ToList())
            {
                var lineHistory = line.CreateMapped<PartContentLine, PartContentLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.PartId = line.Part.Id;
                lineHistory.ContentId = line.Content.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = IdentityUser.Id;
                _repositoryPartContentLineHistory.Add(lineHistory, true);
                _repositoryPartContentLine.Delete(line, true);
            }

            var counterIdCodeNameSelected = 1;

            foreach (var idCodeNameSelected in updateModel.Item.Contents)
            {
                var itemContent = _repositoryContent.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryPartContentLine.Add(new PartContentLine
                {
                    Id = GuidHelper.NewGuid(),
                    Content = itemContent,
                    Part = affectedItem,
                    Creator = IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = counterIdCodeNameSelected,
                    LastModifier = IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<PartContentLine, PartContentLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.PartId = affectedLine.Part.Id;
                lineHistory.ContentId = affectedLine.Content.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryPartContentLineHistory.Add(lineHistory, true);
                counterIdCodeNameSelected++;
            }

           
            //foreach (var idCodeNameSelected in updateModel.Item.Contents)
            //{
            //    var line = _repositoryPartContentLine.Get(x => x.Part.Id == item.Id && x.Content.Code == idCodeNameSelected.Code);
            //    line.DisplayOrder = counterIdCodeNameSelected;
            //    _repositoryPartContentLine.Update(line, true);
            //    counterIdCodeNameSelected++;
            //}



            modelItem.PartId = affectedItem.Id;
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);

            updateModel.Item = modelItem;

            return updateModel;
        }

        public void Delete(Guid id)
        {
            Delete(id, _defaultLanguage.Id);
        }

        public void Delete(Guid partId, Guid languageId)
        {
            // diğer dillerde kayıt yoksa kategori de silinecek

            var line = _repositoryPartLanguageLine.Get(x => x.Part.Id == partId && x.Language.Id == languageId);
            if (line == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var item = _repositoryPart.Get(x => x.Id == partId);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var lineHistory = line.CreateMapped<PartLanguageLine, PartLanguageLineHistory>();

            var versionLine = line.Version;

            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = line.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.PartId = item.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.Version = versionLine + 1;

            _repositoryPartLanguageLineHistory.Add(lineHistory, true);
            _repositoryPartLanguageLine.Delete(line, true);

            if (_repositoryPartLanguageLine.Get().Any(x => x.Part.Id == partId)) return;
            {
                if (_repositoryPartContentLine.Get().Count(x => x.Part.Id == partId) > 0)
                {
                    throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
                }

                var itemHistory = item.CreateMapped<Part, PartHistory>();

                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreationTime = DateTime.Now;
                itemHistory.CreatorId = IdentityUser.Id;
                itemHistory.IsDeleted = true;

                _repositoryPartHistory.Add(itemHistory, true);
                _repositoryPart.Delete(item, true);
            }
        }
    }
}