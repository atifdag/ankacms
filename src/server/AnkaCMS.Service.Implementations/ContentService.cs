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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using AnkaCMS.Service.Implementations.ValidationRules.FluentValidation;

namespace AnkaCMS.Service.Implementations
{
    public class ContentService : IContentService
    {

        private readonly IRepository<Content> _repositoryContent;
        private readonly IRepository<ContentHistory> _repositoryContentHistory;
        private readonly IRepository<ContentLanguageLine> _repositoryContentLanguageLine;
        private readonly IRepository<ContentLanguageLineHistory> _repositoryContentLanguageLineHistory;
        private readonly IRepository<Language> _repositoryLanguage;
        private readonly IRepository<PartContentLine> _repositoryPartContentLine;
        private readonly IRepository<Category> _repositoryCategory;
        private readonly IRepository<User> _repositoryUser;
        private readonly IMainService _serviceMain;
        private readonly Language _defaultLanguage;

        public ContentService(IRepository<Content> repositoryContent, IRepository<ContentHistory> repositoryContentHistory, IRepository<Language> repositoryLanguage, IRepository<User> repositoryUser, IRepository<ContentLanguageLine> repositoryContentLanguageLine, IRepository<ContentLanguageLineHistory> repositoryContentLanguageLineHistory, IRepository<PartContentLine> repositoryPartContentLine, IRepository<Category> repositoryCategory, IMainService serviceMain)
        {
            _repositoryContent = repositoryContent;
            _repositoryContentHistory = repositoryContentHistory;
            _repositoryLanguage = repositoryLanguage;
            _repositoryUser = repositoryUser;

            _repositoryContentLanguageLine = repositoryContentLanguageLine;
            _repositoryContentLanguageLineHistory = repositoryContentLanguageLineHistory;
            _repositoryPartContentLine = repositoryPartContentLine;
            _repositoryCategory = repositoryCategory;
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


        public ListModel<ContentModel> List(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<ContentModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, Guid.Empty, Guid.Empty, model);
        }

        public PublicContentModel PublicDetail(string code)
        {
            var line = _repositoryContentLanguageLine
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x=>x.Language)
                .Join(x=>x.Content)
                .ThenJoin(x=>x.Category)
                .ThenJoin(x=>x.CategoryLanguageLines)
                .ThenJoin(x=>x.Language)
                .FirstOrDefault(x => x.Code == code && x.IsApproved && x.Language.DisplayOrder==1);
            if (line == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var model = line.CreateMapped<ContentLanguageLine, PublicContentModel>();

            model.Category = new IdCodeName(line.Content.Category.Id, line.Content.Category.Code, line.Content.Category.CategoryLanguageLines.FirstOrDefault(x=>x.Language.Code==line.Language.Code).Name);

            model.Language = new IdCodeName(line.Language.Id, line.Language.Code, line.Language.Name);


            return model;
        }

        public ListModel<ContentModel> List(FilterModelWithLanguageAndParent filterModel)
        {
            var model = filterModel.CreateMapped<FilterModelWithLanguage, ListModel<ContentModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Language.Id, filterModel.Parent.Id, model);
        }

        public DetailModel<ContentModel> Detail(Guid id)
        {
            return Detail(id, _defaultLanguage.Id);
        }


        public DetailModel<ContentModel> Detail(Guid contentId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == contentId);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            //////


            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                modelItem = itemLine != null ? itemLine.CreateMapped<ContentLanguageLine, ContentModel>() : new ContentModel();
            }

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.ContentId = item.Id;

            return new DetailModel<ContentModel>
            {
                Item = modelItem
            };
        }

        public List<IdCodeName> List(Guid languageId)
        {
            var list = _repositoryContentLanguageLine.Get().Where(x => x.Language.Id == languageId).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id, x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }
            throw new NotFoundException(Messages.DangerRecordNotFound);
        }

        public ListModel<ContentModel> MyContentList(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<ContentModel>>();
            return MyContentList(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, Guid.Empty, Guid.Empty, model);
        }

        public DetailModel<ContentModel> MyContentDetail(Guid contentId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == contentId && x.Creator.Id == IdentityUser.Id);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            //////


            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                modelItem = itemLine != null ? itemLine.CreateMapped<ContentLanguageLine, ContentModel>() : new ContentModel();
            }

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.ContentId = item.Id;

            return new DetailModel<ContentModel>
            {
                Item = modelItem
            };
        }

        public AddModel<ContentModel> MyContentAdd()
        {
            return new AddModel<ContentModel>
            {
                Item = new ContentModel
                {
                    IsApproved = false
                }
            };
        }

        public AddModel<ContentModel> MyContentAdd(AddModel<ContentModel> addModel)
        {
            IValidator validator = new FluentValidator<ContentModel, ContentValidationRules>(addModel.Item);
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

            var parent = _repositoryCategory.Get(e => e.Id == addModel.Item.Category.Id);

            if (parent == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            if (addModel.Item.File != null)
            {
                addModel.Item.ImageFileLength = addModel.Item.File.Size;
                addModel.Item.ImageFileType = addModel.Item.File.Type;
                addModel.Item.ImageName = addModel.Item.File.Name;
            }

            var line = addModel.Item.CreateMapped<ContentModel, ContentLanguageLine>();

            if (_repositoryContentLanguageLine.Get().FirstOrDefault(e => e.Code == line.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            var item = new Content
            {
                Code = GuidHelper.NewGuid().ToString(),
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                Creator = IdentityUser,
                LastModificationTime = DateTime.Now,
                LastModifier = IdentityUser,
                Category = parent
            };

            var affectedItem = _repositoryContent.Add(item, true);

            var rootPath = AppContext.BaseDirectory;

            if (AppContext.BaseDirectory.Contains("bin"))
            {
                rootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
            }
            var wwwrootPath = Path.Combine(rootPath, "wwwroot");
            var contentFiles = Path.Combine(wwwrootPath, "ContentFiles");
            var publicContentFiles = Path.Combine(contentFiles, "Public");

            var contentDirectoryPath = Path.Combine(contentFiles, affectedItem.Id.ToString());
            if (!Directory.Exists(contentDirectoryPath))
            {
                Directory.CreateDirectory(contentDirectoryPath);
                FileHelper.CopyDirectory(publicContentFiles, contentDirectoryPath);
            }

            if (addModel.Item.ImageFileLength > 0)
            {
                string fileExtension;

                if (addModel.Item.ImageFileType.Contains("jpeg"))
                {
                    fileExtension = ".jpg";
                }
                else if (addModel.Item.ImageFileType.Contains("png"))
                {
                    fileExtension = ".png";
                }
                else
                {
                    fileExtension = ".jpg";
                }

                var newImageName = addModel.Item.Name.ToStringForSeo() + fileExtension;

                ImageHelper.SaveImageFromBase64String(addModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));


                line.ImageName = newImageName;

                line.ImagePath = "/ContentFiles/" + affectedItem.Id + "/" + newImageName;
            }

            var itemHistory = affectedItem.CreateMapped<Content, ContentHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CategoryId = affectedItem.Category.Id;

            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryContentHistory.Add(itemHistory, true);

            var maxLineDisplayOrder = _repositoryContentLanguageLine.Get().Where(x => x.Language.Id == addModel.Item.Language.Id).Max(e => e.DisplayOrder);

            line.Id = GuidHelper.NewGuid();
            line.Version = 1;
            line.DisplayOrder = maxLineDisplayOrder + 1;
            line.CreationTime = DateTime.Now;
            line.Language = language;
            line.Content = affectedItem;
            line.LastModificationTime = DateTime.Now;
            line.Creator = IdentityUser;
            line.LastModifier = IdentityUser;
            line.ViewCount = 0;
            var affectedLine = _repositoryContentLanguageLine.Add(line, true);

            var lineHistory = affectedLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = affectedLine.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.ContentId = affectedLine.Content.Id;
            lineHistory.LanguageId = affectedLine.Language.Id;

            _repositoryContentLanguageLineHistory.Add(lineHistory, true);

            addModel.Item = affectedItem.CreateMapped<Content, ContentModel>();



            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdCodeName(language.Id,language.Code, language.Name);
            addModel.Item.Category = new IdCodeName(parent.Id,parent.Code, parent.Code);
            return addModel;
        }

        public UpdateModel<ContentModel> MyContentUpdate(Guid contentId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == contentId && x.Creator.Id == IdentityUser.Id);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                if (itemLine != null)
                {
                    modelItem = itemLine.CreateMapped<ContentLanguageLine, ContentModel>();
                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
                    modelItem.LastModifier = new IdCodeName(itemLine.LastModifier.Id,itemLine.LastModifier.Username, itemLine.LastModifier.Person.DisplayName);
                }
                else
                {
                    modelItem = new ContentModel();
                }
            }


            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.ContentId = item.Id;

            return new UpdateModel<ContentModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<ContentModel> MyContentUpdate(UpdateModel<ContentModel> updateModel)
        {
            IValidator validator = new FluentValidator<ContentModel, ContentValidationRules>(updateModel.Item);
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

            var parent =
                _repositoryCategory.Get(x => x.Id == updateModel.Item.Category.Id);

            if (parent == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == updateModel.Item.ContentId && x.Creator.Id == IdentityUser.Id);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);


                if (updateModel.Item.File != null)
                {
                    updateModel.Item.ImageFileLength = updateModel.Item.File.Size;
                    updateModel.Item.ImageFileType = updateModel.Item.File.Type;
                    updateModel.Item.ImageName = updateModel.Item.File.Name;
                }

                var rootPath = AppContext.BaseDirectory;

                if (AppContext.BaseDirectory.Contains("bin"))
                {
                    rootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                }
                var wwwrootPath = Path.Combine(rootPath, "wwwroot");
                var contentFiles = Path.Combine(wwwrootPath, "ContentFiles");

                var contentDirectoryPath = Path.Combine(contentFiles, item.Id.ToString());

                // güncelleme yapılacak
                if (itemLine != null)
                {
                    var version = itemLine.Version;
                    var lineHistory = itemLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = itemLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.ContentId = item.Id;

                    lineHistory.LanguageId = language.Id;
                    _repositoryContentLanguageLineHistory.Add(lineHistory, true);


                    if (updateModel.Item.File != null)
                    {
                        if (updateModel.Item.ImageFileLength > 0)
                        {

                            if (itemLine.ImageName != null)
                            {
                                var oldFilePath = Path.Combine(contentDirectoryPath, itemLine.ImageName);
                                // dosya varsa sil
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }



                            string fileExtension;

                            if (updateModel.Item.ImageFileType.Contains("jpeg"))
                            {
                                fileExtension = ".jpg";
                            }
                            else if (updateModel.Item.ImageFileType.Contains("png"))
                            {
                                fileExtension = ".png";
                            }
                            else
                            {
                                fileExtension = ".jpg";
                            }

                            var newImageName = updateModel.Item.Name.ToStringForSeo() + fileExtension;

                            ImageHelper.SaveImageFromBase64String(updateModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));

                            itemLine.ImageName = newImageName;
                            itemLine.ImagePath = "/ContentFiles/" + item.Id + "/" + newImageName;
                            itemLine.ImageFileType = updateModel.Item.ImageFileType;
                            itemLine.ImageFileLength = updateModel.Item.ImageFileLength;


                        }

                    }

                    itemLine.Code = updateModel.Item.Code;
                    itemLine.Name = updateModel.Item.Name;
                    itemLine.ShortName = updateModel.Item.ShortName;
                    itemLine.Description = updateModel.Item.Description;
                    itemLine.Keywords = updateModel.Item.Keywords;
                    itemLine.ContentDetail = updateModel.Item.ContentDetail;
                    itemLine.Url = updateModel.Item.Url;


                    itemLine.Version = version + 1;

                    itemLine.IsApproved = updateModel.Item.IsApproved;
                    itemLine.Content = item;
                    itemLine.LastModifier = IdentityUser;
                    itemLine.LastModificationTime = DateTime.Now;
                    var affectedItemLine = _repositoryContentLanguageLine.Update(itemLine, true);
                    modelItem = affectedItemLine.CreateMapped<ContentLanguageLine, ContentModel>();


                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);

                }

                // ekleme yapılacak
                else
                {
                    modelItem = new ContentModel();

                    var maxLineDisplayOrder = _repositoryContentLanguageLine.Get().Where(x => x.Language.Id == language.Id).Max(e => e.DisplayOrder);

                    var line = updateModel.Item.CreateMapped<ContentModel, ContentLanguageLine>();

                    line.Id = GuidHelper.NewGuid();
                    line.Version = 1;
                    line.DisplayOrder = maxLineDisplayOrder + 1;
                    line.CreationTime = DateTime.Now;
                    line.Language = language;
                    line.Content = item;
                    line.LastModificationTime = DateTime.Now;
                    line.Creator = IdentityUser;
                    line.LastModifier = IdentityUser;
                    line.ViewCount = 0;


                    if (updateModel.Item.File != null)
                    {
                        if (updateModel.Item.ImageFileLength > 0)
                        {
                            string fileExtension;

                            if (updateModel.Item.ImageFileType.Contains("jpeg"))
                            {
                                fileExtension = ".jpg";
                            }
                            else if (updateModel.Item.ImageFileType.Contains("png"))
                            {
                                fileExtension = ".png";
                            }
                            else
                            {
                                fileExtension = ".jpg";
                            }

                            var newImageName = updateModel.Item.Name.ToStringForSeo() + fileExtension;

                            ImageHelper.SaveImageFromBase64String(updateModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));

                            line.ImageName = newImageName;
                            line.ImagePath = "/ContentFiles/" + item.Id + "/" + newImageName;
                            line.ImageFileType = updateModel.Item.ImageFileType;
                            line.ImageFileLength = updateModel.Item.ImageFileLength;
                        }
                    }

                    var affectedLine = _repositoryContentLanguageLine.Add(line, true);

                    var lineHistory = affectedLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = affectedLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.ContentId = affectedLine.Content.Id;
                    lineHistory.LanguageId = affectedLine.Language.Id;

                    _repositoryContentLanguageLineHistory.Add(lineHistory, true);


                }
            }

            var itemHistory = item.CreateMapped<Content, ContentHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CategoryId = item.Category.Id;
            itemHistory.CreatorId = IdentityUser.Id;

            _repositoryContentHistory.Add(itemHistory, true);
            item.LastModificationTime = DateTime.Now;

            item.Category = parent;
            item.LastModifier = IdentityUser;
            var affectedItem = _repositoryContent.Update(item, true);

            modelItem.ContentId = affectedItem.Id;
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.Category = new IdCodeName(parent.Id,parent.Code, parent.Code);

            updateModel.Item = modelItem;

            return updateModel;
        }

        public void MyContentDelete(Guid contentId, Guid languageId)
        {
            var item = _repositoryContent
                .Join(x => x.Category)
                .FirstOrDefault(x => x.Id == contentId && x.Creator.Id == IdentityUser.Id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (_repositoryPartContentLine.Get().Count(x => x.Content.Id == contentId) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }


            // diğer dillerde kayıt yoksa içerik de silinecek

            var line = _repositoryContentLanguageLine.Get(x => x.Content.Id == contentId && x.Language.Id == languageId);
            if (line != null)
            {
                var lineHistory = line.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();

                var versionLine = line.Version;

                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.ContentId = item.Id;
                lineHistory.CreatorId = IdentityUser.Id;
                lineHistory.Version = versionLine + 1;

                _repositoryContentLanguageLineHistory.Add(lineHistory, true);
                _repositoryContentLanguageLine.Delete(line, true);
            }


            if (_repositoryContentLanguageLine.Get().Any(x => x.Content.Id == contentId)) return;
            var itemHistory = item.CreateMapped<Content, ContentHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CategoryId = item.Category.Id;
            itemHistory.CreationTime = DateTime.Now;
            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.IsDeleted = true;

            _repositoryContentHistory.Add(itemHistory, true);
            _repositoryContent.Delete(item, true);
        }

        private ListModel<ContentModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid languageId, Guid parentId, ListModel<ContentModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();
            var language = languageId != Guid.Empty ? _repositoryLanguage.Get(x => x.Id == languageId) : _defaultLanguage;

            Expression<Func<Content, bool>> expression;

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
                        expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved == false);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.IsApproved == false);
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched));
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            if (parentId != Guid.Empty)
            {
                expression = expression.And(e => e.Category.Id == parentId);
            }

            var sortHelper = new SortHelper<ContentModel>();

            var query = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)

                .ThenJoin(x => x.Language)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<ContentModel>();
            foreach (var item in items)
            {
                ContentModel modelItem;
                if (item.ContentLanguageLines == null)
                {
                    modelItem = new ContentModel();
                }
                else
                {
                    var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    modelItem = itemLine != null ? itemLine.CreateMapped<ContentLanguageLine, ContentModel>() : new ContentModel();
                }

                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
                modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
                modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
                modelItem.ContentId = item.Id;
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


        private ListModel<ContentModel> MyContentList(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, Guid languageId, Guid parentId, ListModel<ContentModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();
            var language = languageId != Guid.Empty ? _repositoryLanguage.Get(x => x.Id == languageId) : _defaultLanguage;

            Expression<Func<Content, bool>> expression;

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
                        expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched) && x.IsApproved == false);
                    }
                }
                else
                {
                    if (bStatus)
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.IsApproved);
                    }
                    else
                    {
                        expression = c => c.ContentLanguageLines.Any(x => x.IsApproved == false);
                    }
                }

            }
            else
            {
                if (searched != null)
                {
                    expression = c => c.ContentLanguageLines.Any(x => x.Name.Contains(searched));
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            if (parentId != Guid.Empty)
            {
                expression = expression.And(e => e.Category.Id == parentId);
            }

            expression = expression.And(x => x.Creator.Id == IdentityUser.Id);

            var sortHelper = new SortHelper<ContentModel>();

            var query = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)

                .ThenJoin(x => x.Language)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<ContentModel>();
            foreach (var item in items)
            {
                ContentModel modelItem;
                if (item.ContentLanguageLines == null)
                {
                    modelItem = new ContentModel();
                }
                else
                {
                    var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);
                    modelItem = itemLine != null ? itemLine.CreateMapped<ContentLanguageLine, ContentModel>() : new ContentModel();
                }

                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
                modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
                modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
                modelItem.ContentId = item.Id;
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


        public AddModel<ContentModel> Add()
        {
            return new AddModel<ContentModel>
            {
                Item = new ContentModel
                {
                    IsApproved = false
                }
            };
        }

        public AddModel<ContentModel> Add(AddModel<ContentModel> addModel)
        {
            IValidator validator = new FluentValidator<ContentModel, ContentValidationRules>(addModel.Item);
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

            var parent = _repositoryCategory.Get(e => e.Id == addModel.Item.Category.Id);

            if (parent == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            if (addModel.Item.File != null)
            {
                addModel.Item.ImageFileLength = addModel.Item.File.Size;
                addModel.Item.ImageFileType = addModel.Item.File.Type;
                addModel.Item.ImageName = addModel.Item.File.Name;
            }

            var line = addModel.Item.CreateMapped<ContentModel, ContentLanguageLine>();

            if (_repositoryContentLanguageLine.Get().FirstOrDefault(e => e.Code == line.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }

            var item = new Content
            {
                Code = GuidHelper.NewGuid().ToString(),
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                Creator = IdentityUser,
                LastModificationTime = DateTime.Now,
                LastModifier = IdentityUser,
                Category = parent
            };

            var affectedItem = _repositoryContent.Add(item, true);

            var rootPath = AppContext.BaseDirectory;

            if (AppContext.BaseDirectory.Contains("bin"))
            {
                rootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
            }



            var wwwrootPath = Path.Combine(rootPath, "wwwroot");
            var contentFiles = Path.Combine(wwwrootPath, "ContentFiles");
            var publicContentFiles = Path.Combine(contentFiles, "Public");

            var contentDirectoryPath = Path.Combine(contentFiles, affectedItem.Id.ToString());
            if (!Directory.Exists(contentDirectoryPath))
            {
                Directory.CreateDirectory(contentDirectoryPath);
                FileHelper.CopyDirectory(publicContentFiles, contentDirectoryPath);
            }

            if (addModel.Item.ImageFileLength > 0)
            {
                string fileExtension;

                if (addModel.Item.ImageFileType.Contains("jpeg"))
                {
                    fileExtension = ".jpg";
                }
                else if (addModel.Item.ImageFileType.Contains("png"))
                {
                    fileExtension = ".png";
                }
                else
                {
                    fileExtension = ".jpg";
                }

                var newImageName = addModel.Item.Name.ToStringForSeo() + fileExtension;

                ImageHelper.SaveImageFromBase64String(addModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));


                line.ImageName = newImageName;

                line.ImagePath = "/ContentFiles/" + affectedItem.Id + "/" + newImageName;
            }

            var itemHistory = affectedItem.CreateMapped<Content, ContentHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CategoryId = affectedItem.Category.Id;

            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryContentHistory.Add(itemHistory, true);

            var maxLineDisplayOrder = _repositoryContentLanguageLine.Get().Where(x => x.Language.Id == addModel.Item.Language.Id).Max(e => e.DisplayOrder);

            line.Id = GuidHelper.NewGuid();
            line.Version = 1;
            line.DisplayOrder = maxLineDisplayOrder + 1;
            line.CreationTime = DateTime.Now;
            line.Language = language;
            line.Content = affectedItem;
            line.LastModificationTime = DateTime.Now;
            line.Creator = IdentityUser;
            line.LastModifier = IdentityUser;
            line.ViewCount = 0;
            var affectedLine = _repositoryContentLanguageLine.Add(line, true);

            var lineHistory = affectedLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
            lineHistory.Id = GuidHelper.NewGuid();
            lineHistory.ReferenceId = affectedLine.Id;
            lineHistory.CreatorId = IdentityUser.Id;
            lineHistory.CreationTime = DateTime.Now;
            lineHistory.ContentId = affectedLine.Content.Id;
            lineHistory.LanguageId = affectedLine.Language.Id;

            _repositoryContentLanguageLineHistory.Add(lineHistory, true);

            addModel.Item = affectedItem.CreateMapped<Content, ContentModel>();



            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.Language = new IdCodeName(language.Id,language.Code, language.Name);
            addModel.Item.Category = new IdCodeName(parent.Id,parent.Code, parent.Code);
            return addModel;
        }

        public UpdateModel<ContentModel> Update(Guid id)
        {
            return Update(id, _defaultLanguage.Id);
        }

        public UpdateModel<ContentModel> Update(Guid contentId, Guid languageId)
        {
            var language = _repositoryLanguage.Get(x => x.Id == languageId);

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == contentId);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == languageId);
                if (itemLine != null)
                {
                    modelItem = itemLine.CreateMapped<ContentLanguageLine, ContentModel>();
                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.Category = new IdCodeName(item.Category.Id, item.Category.Code, item.Category.Code);
                    modelItem.LastModifier = new IdCodeName(itemLine.LastModifier.Id,itemLine.LastModifier.Username, itemLine.LastModifier.Person.DisplayName);
                }
                else
                {
                    modelItem = new ContentModel();
                }
            }


            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.ContentId = item.Id;

            return new UpdateModel<ContentModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<ContentModel> Update(UpdateModel<ContentModel> updateModel)
        {
            IValidator validator = new FluentValidator<ContentModel, ContentValidationRules>(updateModel.Item);
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

            var parent =
                _repositoryCategory.Get(x => x.Id == updateModel.Item.Category.Id);

            if (parent == null)
            {
                throw new NotFoundException(Messages.DangerParentNotFound);
            }

            var item = _repositoryContent
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.Category)
                .Join(z => z.ContentLanguageLines)
                .ThenJoin(x => x.Language)
                .FirstOrDefault(x => x.Id == updateModel.Item.ContentId);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            ContentModel modelItem;
            if (item.ContentLanguageLines == null)
            {
                modelItem = new ContentModel();
            }
            else
            {
                var itemLine = item.ContentLanguageLines.FirstOrDefault(x => x.Language.Id == language.Id);


                if (updateModel.Item.File != null)
                {
                    updateModel.Item.ImageFileLength = updateModel.Item.File.Size;
                    updateModel.Item.ImageFileType = updateModel.Item.File.Type;
                    updateModel.Item.ImageName = updateModel.Item.File.Name;
                }

                var rootPath = AppContext.BaseDirectory;

                if (AppContext.BaseDirectory.Contains("bin"))
                {
                    rootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                }
                var wwwrootPath = Path.Combine(rootPath, "wwwroot");
                var contentFiles = Path.Combine(wwwrootPath, "ContentFiles");

                var contentDirectoryPath = Path.Combine(contentFiles, item.Id.ToString());

                // güncelleme yapılacak
                if (itemLine != null)
                {
                    var version = itemLine.Version;
                    var lineHistory = itemLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = itemLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.ContentId = item.Id;

                    lineHistory.LanguageId = language.Id;
                    _repositoryContentLanguageLineHistory.Add(lineHistory, true);


                    if (updateModel.Item.File != null)
                    {
                        if (updateModel.Item.ImageFileLength > 0)
                        {

                            if (itemLine.ImageName != null)
                            {
                                var oldFilePath = Path.Combine(contentDirectoryPath, itemLine.ImageName);
                                // dosya varsa sil
                                if (System.IO.File.Exists(oldFilePath))
                                {
                                    System.IO.File.Delete(oldFilePath);
                                }
                            }



                            string fileExtension;

                            if (updateModel.Item.ImageFileType.Contains("jpeg"))
                            {
                                fileExtension = ".jpg";
                            }
                            else if (updateModel.Item.ImageFileType.Contains("png"))
                            {
                                fileExtension = ".png";
                            }
                            else
                            {
                                fileExtension = ".jpg";
                            }

                            var newImageName = updateModel.Item.Name.ToStringForSeo() + fileExtension;

                            ImageHelper.SaveImageFromBase64String(updateModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));

                            itemLine.ImageName = newImageName;
                            itemLine.ImagePath = "/ContentFiles/" + item.Id + "/" + newImageName;
                            itemLine.ImageFileType = updateModel.Item.ImageFileType;
                            itemLine.ImageFileLength = updateModel.Item.ImageFileLength;


                        }

                    }





                    itemLine.Code = updateModel.Item.Code;
                    itemLine.Name = updateModel.Item.Name;
                    itemLine.ShortName = updateModel.Item.ShortName;
                    itemLine.Description = updateModel.Item.Description;
                    itemLine.Keywords = updateModel.Item.Keywords;
                    itemLine.ContentDetail = updateModel.Item.ContentDetail;
                    itemLine.Url = updateModel.Item.Url;


                    itemLine.Version = version + 1;

                    itemLine.IsApproved = updateModel.Item.IsApproved;
                    itemLine.Content = item;
                    itemLine.LastModifier = IdentityUser;
                    itemLine.LastModificationTime = DateTime.Now;
                    var affectedItemLine = _repositoryContentLanguageLine.Update(itemLine, true);
                    modelItem = affectedItemLine.CreateMapped<ContentLanguageLine, ContentModel>();


                    modelItem.Creator = new IdCodeName(itemLine.Creator.Id,itemLine.Creator.Username, itemLine.Creator.Person.DisplayName);
                    modelItem.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);








                }

                // ekleme yapılacak
                else
                {
                    modelItem = new ContentModel();

                    var maxLineDisplayOrder = _repositoryContentLanguageLine.Get().Where(x => x.Language.Id == language.Id).Max(e => e.DisplayOrder);

                    var line = updateModel.Item.CreateMapped<ContentModel, ContentLanguageLine>();

                    line.Id = GuidHelper.NewGuid();
                    line.Version = 1;
                    line.DisplayOrder = maxLineDisplayOrder + 1;
                    line.CreationTime = DateTime.Now;
                    line.Language = language;
                    line.Content = item;
                    line.LastModificationTime = DateTime.Now;
                    line.Creator = IdentityUser;
                    line.LastModifier = IdentityUser;
                    line.ViewCount = 0;


                    if (updateModel.Item.File != null)
                    {
                        if (updateModel.Item.ImageFileLength > 0)
                        {
                            string fileExtension;

                            if (updateModel.Item.ImageFileType.Contains("jpeg"))
                            {
                                fileExtension = ".jpg";
                            }
                            else if (updateModel.Item.ImageFileType.Contains("png"))
                            {
                                fileExtension = ".png";
                            }
                            else
                            {
                                fileExtension = ".jpg";
                            }

                            var newImageName = updateModel.Item.Name.ToStringForSeo() + fileExtension;

                            ImageHelper.SaveImageFromBase64String(updateModel.Item.File.Value.Split(",")[1], Path.Combine(contentDirectoryPath, newImageName));

                            line.ImageName = newImageName;
                            line.ImagePath = "/ContentFiles/" + item.Id + "/" + newImageName;
                            line.ImageFileType = updateModel.Item.ImageFileType;
                            line.ImageFileLength = updateModel.Item.ImageFileLength;
                        }
                    }

                    var affectedLine = _repositoryContentLanguageLine.Add(line, true);

                    var lineHistory = affectedLine.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = affectedLine.Id;
                    lineHistory.CreatorId = IdentityUser.Id;
                    lineHistory.CreationTime = DateTime.Now;
                    lineHistory.ContentId = affectedLine.Content.Id;
                    lineHistory.LanguageId = affectedLine.Language.Id;

                    _repositoryContentLanguageLineHistory.Add(lineHistory, true);


                }
            }

            var itemHistory = item.CreateMapped<Content, ContentHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CategoryId = item.Category.Id;
            itemHistory.CreatorId = IdentityUser.Id;

            _repositoryContentHistory.Add(itemHistory, true);
            item.LastModificationTime = DateTime.Now;

            item.Category = parent;
            item.LastModifier = IdentityUser;
            var affectedItem = _repositoryContent.Update(item, true);

            modelItem.ContentId = affectedItem.Id;
            modelItem.Language = new IdCodeName(language.Id,language.Code, language.Name);
            modelItem.Category = new IdCodeName(parent.Id,parent.Code, parent.Code);

            updateModel.Item = modelItem;

            return updateModel;
        }

        public void Delete(Guid id)
        {
            Delete(id, _defaultLanguage.Id);
        }

        public void Delete(Guid contentId, Guid languageId)
        {

            var item = _repositoryContent
                .Join(x => x.Category)
                .FirstOrDefault(x => x.Id == contentId);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (_repositoryPartContentLine.Get().Count(x => x.Content.Id == contentId) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }


            // diğer dillerde kayıt yoksa içerik de silinecek

            var line = _repositoryContentLanguageLine.Get(x => x.Content.Id == contentId && x.Language.Id == languageId);
            if (line != null)
            {
                var lineHistory = line.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();

                var versionLine = line.Version;

                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.ContentId = item.Id;
                lineHistory.CreatorId = IdentityUser.Id;
                lineHistory.Version = versionLine + 1;

                _repositoryContentLanguageLineHistory.Add(lineHistory, true);
                _repositoryContentLanguageLine.Delete(line, true);
            }


            if (_repositoryContentLanguageLine.Get().Any(x => x.Content.Id == contentId)) return;
            var itemHistory = item.CreateMapped<Content, ContentHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CategoryId = item.Category.Id;
            itemHistory.CreationTime = DateTime.Now;
            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.IsDeleted = true;

            _repositoryContentHistory.Add(itemHistory, true);
            _repositoryContent.Delete(item, true);
        }
    }
}