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
    public class CategoryService : ICategoryService
    {

        private readonly IRepository<Category> _repositoryCategory;
        private readonly IRepository<CategoryHistory> _repositoryCategoryHistory;
        private readonly IRepository<CategoryLanguageLine> _repositoryCategoryLanguageLine;
        private readonly IRepository<CategoryLanguageLineHistory> _repositoryCategoryLanguageLineHistory;
        private readonly IRepository<Language> _repositoryLanguage;
        private readonly IRepository<Content> _repositoryContent;
        private readonly IRepository<User> _repositoryUser;

        private readonly Language _defaultLanguage;
        private readonly IMainService _serviceMain;

        public CategoryService(IRepository<Category> repositoryCategory, IRepository<CategoryHistory> repositoryCategoryHistory, IRepository<Language> repositoryLanguage, IRepository<User> repositoryUser, IRepository<CategoryLanguageLine> repositoryCategoryLanguageLine, IRepository<CategoryLanguageLineHistory> repositoryCategoryLanguageLineHistory, IRepository<Content> repositoryContent, IMainService serviceMain)
        {
            _repositoryCategory = repositoryCategory;
            _repositoryCategoryHistory = repositoryCategoryHistory;
            _repositoryLanguage = repositoryLanguage;
            _repositoryUser = repositoryUser;

            _repositoryCategoryLanguageLine = repositoryCategoryLanguageLine;
            _repositoryCategoryLanguageLineHistory = repositoryCategoryLanguageLineHistory;
            _repositoryContent = repositoryContent;
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


        public ListModel<CategoryModel> List(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<CategoryModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, Guid.Empty, model);
        }

        public ListModel<CategoryModel> List(FilterModelWithLanguage filterModel)
        {
            var model = filterModel.CreateMapped<FilterModelWithLanguage, ListModel<CategoryModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Language.Id, model);
        }

        public DetailModel<CategoryModel> Detail(Guid id)
        {
            return Detail(id, _defaultLanguage.Id);
        }


        public DetailModel<CategoryModel> Detail(Guid categoryId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == categoryId);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            //////


            CategoryModel modelItem;
            if (item.CategoryLanguageLines == null)
            {
                modelItem = new CategoryModel();
            }
            else
            {
                var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                modelItem = itemLine != null ? itemLine.CreateMapped<CategoryLanguageLine, CategoryModel>() : new CategoryModel();
            }

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);
            modelItem.CategoryId = item.Id;

            return new DetailModel<CategoryModel>
            {
                Item = modelItem
            };
        }

        public List<IdCodeName> List(Guid languageId)
        {
            var list = _repositoryCategoryLanguageLine.Get().Where(x => x.Language.Id == languageId).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id, x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }
            throw new NotFoundException(Messages.DangerRecordNotFound);
        }

        private ListModel<CategoryModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid languageId, ListModel<CategoryModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();
            var language = languageId != Guid.Empty ? _repositoryLanguage.Get(x => x.Id == languageId) : _defaultLanguage;

            Expression<Func<Category, bool>> expression;

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
                        expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved == false);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.CategoryLanguageLines.Any(x => x.IsApproved == false);
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.CategoryLanguageLines.Any(x => x.Name.Contains(searched));
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            var sortHelper = new SortHelper<CategoryModel>();

            var query = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<CategoryModel>();
            foreach (var item in items)
            {
                CategoryModel modelItem;
                if (item.CategoryLanguageLines == null)
                {
                    modelItem = new CategoryModel();
                }
                else
                {
                    var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    modelItem = itemLine != null ? itemLine.CreateMapped<CategoryLanguageLine, CategoryModel>() : new CategoryModel();
                }

                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id, item.LastModifier.Username, item.LastModifier.Person.DisplayName);
                modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);
                modelItem.CategoryId = item.Id;
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

        public AddModel<CategoryModel> Add()
        {
            return new AddModel<CategoryModel>
            {
                Item = new CategoryModel
                {
                    IsApproved = false
                }
            };
        }

        public AddModel<CategoryModel> Add(AddModel<CategoryModel> addModel)
        {
            IValidator validator = new FluentValidator<CategoryModel, CategoryValidationRules>(addModel.Item);
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

            var line = addModel.Item.CreateMapped<CategoryModel, CategoryLanguageLine>();

            if (_repositoryCategoryLanguageLine.Get().FirstOrDefault(e => e.Code == line.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            var item = new Category
            {
                Code = GuidHelper.NewGuid().ToString(),
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                Creator = IdentityUser,
                LastModificationTime = DateTime.Now,
                LastModifier = IdentityUser
            };

            var affectedItem = _repositoryCategory.Add(item, true);
            var itemHistory = affectedItem.CreateMapped<Category, CategoryHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryCategoryHistory.Add(itemHistory, true);

            var maxLineDisplayOrder = _repositoryCategoryLanguageLine.Get().Where(x => x.Language.Id == addModel.Item.Language.Id).Max(e => e.DisplayOrder);

            line.Id = GuidHelper.NewGuid();
            line.Version = 1;
            line.DisplayOrder = maxLineDisplayOrder + 1;
            line.CreationTime = DateTime.Now;
            line.Language = language;
            line.Category = affectedItem;
            line.LastModificationTime = DateTime.Now;
            line.Creator = IdentityUser;
            line.LastModifier = IdentityUser;
            var affectedLine = _repositoryCategoryLanguageLine.Add(line, true);

            var lineHistory = affectedLine.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = affectedLine.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.CategoryId = affectedLine.Category.Id;
            lineHistory.LanguageId = affectedLine.Language.Id;

            _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);

            addModel.Item = affectedItem.CreateMapped<Category, CategoryModel>();



            addModel.Item.Creator = new IdCodeName(IdentityUser.Id, IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id, IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdCodeName(language.Id, language.Code, language.Name);
            return addModel;
        }

        public UpdateModel<CategoryModel> Update(Guid id)
        {
            return Update(id, _defaultLanguage.Id);
        }

        public UpdateModel<CategoryModel> Update(Guid categoryId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == categoryId);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            CategoryModel modelItem;
            if (item.CategoryLanguageLines == null)
            {
                modelItem = new CategoryModel();
            }
            else
            {
                var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                if (itemLine != null)
                {
                    modelItem = itemLine.CreateMapped<CategoryLanguageLine, CategoryModel>();
                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id, itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(itemLine.LastModifier.Id, itemLine.LastModifier.Username, itemLine.LastModifier.Person.DisplayName);
                }
                else
                {
                    modelItem = new CategoryModel();
                }
            }


            modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);
            modelItem.CategoryId = item.Id;

            return new UpdateModel<CategoryModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<CategoryModel> Update(UpdateModel<CategoryModel> updateModel)
        {
            IValidator validator = new FluentValidator<CategoryModel, CategoryValidationRules>(updateModel.Item);
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

            var item = _repositoryCategory
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(z => z.CategoryLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == updateModel.Item.CategoryId);



            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            CategoryModel modelItem;
            if (item.CategoryLanguageLines == null)
            {
                modelItem = new CategoryModel();
            }
            else
            {
                var itemLine = item.CategoryLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);

                // güncelleme yapılacak
                if (itemLine != null)
                {
                    var version = itemLine.Version;
                    var lineHistory = itemLine.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = itemLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.CategoryId = item.Id;
                    lineHistory.LanguageId = language.Id;
                    _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);

                    itemLine.Code = updateModel.Item.Code;
                    itemLine.Name = updateModel.Item.Name;
                    itemLine.Description = updateModel.Item.Description;
                    itemLine.Keywords = updateModel.Item.Keywords;
                    itemLine.Version = version + 1;

                    itemLine.IsApproved = updateModel.Item.IsApproved;
                    itemLine.Category = item;
                    itemLine.LastModifier = IdentityUser;
                    itemLine.LastModificationTime = DateTime.Now;
                    var affectedItemLine = _repositoryCategoryLanguageLine.Update(itemLine, true);
                    modelItem = affectedItemLine.CreateMapped<CategoryLanguageLine, CategoryModel>();


                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id, itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(IdentityUser.Id, IdentityUser.Username, IdentityUser.Person.DisplayName);

                }

                // ekleme yapılacak
                else
                {
                    modelItem = new CategoryModel();

                    var maxLineDisplayOrder = _repositoryCategoryLanguageLine.Get().Where(x => x.Language.Id == language.Id).Max(e => e.DisplayOrder);

                    var line = updateModel.Item.CreateMapped<CategoryModel, CategoryLanguageLine>();

                    line.Id = GuidHelper.NewGuid();
                    line.Version = 1;
                    line.DisplayOrder = maxLineDisplayOrder + 1;
                    line.CreationTime = DateTime.Now;
                    line.Language = language;
                    line.Category = item;
                    line.LastModificationTime = DateTime.Now;
                    line.Creator = IdentityUser;
                    line.LastModifier = IdentityUser;
                    var affectedLine = _repositoryCategoryLanguageLine.Add(line, true);

                    var lineHistory = affectedLine.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = affectedLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.CategoryId = affectedLine.Category.Id;
                    lineHistory.LanguageId = affectedLine.Language.Id;

                    _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);


                }
            }

            var itemHistory = item.CreateMapped<Category, CategoryHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreatorId = IdentityUser.Id;

            _repositoryCategoryHistory.Add(itemHistory, true);
            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            var affectedItem = _repositoryCategory.Update(item, true);

            modelItem.CategoryId = affectedItem.Id;
            modelItem.Language = new IdCodeName(language.Id, language.Code, language.Name);

            updateModel.Item = modelItem;

            return updateModel;
        }

        public void Delete(Guid id)
        {
            Delete(id, _defaultLanguage.Id);
        }

        public void Delete(Guid categoryId, Guid languageId)
        {
            // diğer dillerde kayıt yoksa kategori de silinecek

            var line = _repositoryCategoryLanguageLine.Get(x => x.Category.Id == categoryId && x.Language.Id == languageId);
            if (line == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var item = _repositoryCategory.Get(x => x.Id == categoryId);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var lineHistory = line.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();

            var versionLine = line.Version;

            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = line.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.CategoryId = item.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.Version = versionLine + 1;

            _repositoryCategoryLanguageLineHistory.Add(lineHistory, true);
            _repositoryCategoryLanguageLine.Delete(line, true);

            if (_repositoryCategoryLanguageLine.Get().Any(x => x.Category.Id == categoryId)) return;
            {
                if (_repositoryContent.Get().Count(x => x.Category.Id == categoryId) > 0)
                {
                    throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
                }

                var itemHistory = item.CreateMapped<Category, CategoryHistory>();

                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreationTime = DateTime.Now;
                itemHistory.CreatorId = IdentityUser.Id;
                itemHistory.IsDeleted = true;

                _repositoryCategoryHistory.Add(itemHistory, true);
                _repositoryCategory.Delete(item, true);
            }
        }

        public List<IdCodeName> List()
        {
            var list = _repositoryCategory.Get().OrderBy(x => x.Code).Select(x => new IdCodeName(x.Id, x.Code, x.Code));
            if (list.Any())
            {
                return list.ToList();
            }
            throw new NotFoundException(Messages.DangerRecordNotFound);
        }
         
        public PublicCategoryModel PublicDetail(string code)
        {
            var lineCategoryLanguage = _repositoryCategoryLanguageLine
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Language)
                .Join(x => x.Category)
                .ThenJoin(x => x.Contents)
                .ThenJoin(x => x.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Code == code && x.IsApproved);
            if (lineCategoryLanguage == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var model = lineCategoryLanguage.CreateMapped<CategoryLanguageLine, PublicCategoryModel>();
            model.Language = new IdCodeName(lineCategoryLanguage.Language.Id, lineCategoryLanguage.Language.Code, lineCategoryLanguage.Language.Name);
            model.CategoryId = lineCategoryLanguage.Category.Id;

            var listPublicContent = new List<PublicContentModel>();

            var contents = lineCategoryLanguage.Category.Contents;

            foreach (var itemContent in contents.Where(a => a.ContentLanguageLines.All(b => b.IsApproved)))
            {
                foreach (var lineContentLanguage in itemContent.ContentLanguageLines.OrderBy(a => a.DisplayOrder).Where(x => x.Language.Id == lineCategoryLanguage.Language.Id))
                {
                    var itemPublicContent = lineContentLanguage.CreateMapped<ContentLanguageLine, PublicContentModel>();
                    itemPublicContent.Category = new IdCodeName(itemContent.Category.Id, itemContent.Category.Code, lineCategoryLanguage.Name);
                    itemPublicContent.Language = new IdCodeName(lineContentLanguage.Language.Id, lineContentLanguage.Language.Code, lineContentLanguage.Language.Name);
                    itemPublicContent.ContentId = itemContent.Id;
                    listPublicContent.Add(itemPublicContent);
                }
            }

            model.Contents = listPublicContent;

            return model;
        }
    }
}