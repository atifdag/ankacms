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
    public class PermissionService : IPermissionService
    {

        private readonly IRepository<Permission> _repositoryPermission;
        private readonly IRepository<PermissionHistory> _repositoryPermissionHistory;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<PermissionMenuLine> _repositoryPermissionMenuLine;
        private readonly IRepository<PermissionMenuLineHistory> _repositoryPermissionMenuLineHistory;
        private readonly IRepository<Menu> _repositoryMenu;
        private readonly IRepository<User> _repositoryUser;
        private readonly IMainService _serviceMain;

        public PermissionService(IRepository<Permission> repositoryPermission, IRepository<PermissionHistory> repositoryPermissionHistory, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<PermissionMenuLine> repositoryPermissionMenuLine, IRepository<User> repositoryUser, IRepository<Menu> repositoryMenu, IRepository<PermissionMenuLineHistory> repositoryPermissionMenuLineHistory, IMainService serviceMain)
        {
            _repositoryPermission = repositoryPermission;
            _repositoryPermissionHistory = repositoryPermissionHistory;
            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryPermissionMenuLine = repositoryPermissionMenuLine;
            _repositoryUser = repositoryUser;

            _repositoryMenu = repositoryMenu;
            _repositoryPermissionMenuLineHistory = repositoryPermissionMenuLineHistory;
            _serviceMain = serviceMain;
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


        public ListModel<PermissionModel> List(FilterModel filterModel)
        {
            var startDate = filterModel.StartDate.ResetTimeToStartOfDay();
            var endDate = filterModel.EndDate.ResetTimeToEndOfDay();
            Expression<Func<Permission, bool>> expression;

            if (filterModel.Status != -1)
            {
                var status = filterModel.Status.ToString().ToBoolean();
                if (filterModel.Searched != null)
                {
                    if (status)
                    {
                        expression = c => c.IsApproved && c.Name.Contains(filterModel.Searched);
                    }
                    else
                    {
                        expression = c => c.IsApproved == false && c.Name.Contains(filterModel.Searched);
                    }
                }
                else
                {
                    if (status)
                    {
                        expression = c => c.IsApproved;
                    }
                    else
                    {
                        expression = c => c.IsApproved == false;
                    }
                }

            }
            else
            {
                if (filterModel.Searched != null)
                {
                    expression = c => c.Name.Contains(filterModel.Searched);
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= startDate && e.CreationTime <= endDate);

            var model = filterModel.CreateMapped<FilterModel, ListModel<PermissionModel>>();

            if (model.Paging == null)
            {
                model.Paging = new Paging
                {
                    PageSize = filterModel.PageSize,
                    PageNumber = filterModel.PageNumber
                };
            }

            var sortHelper = new SortHelper<PermissionModel>();

            var query = _repositoryPermission
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<PermissionModel>();
            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Permission, PermissionModel>();
                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
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

        public DetailModel<PermissionModel> Detail(Guid id)
        {
            var item = _repositoryPermission
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.PermissionMenuLines)
                .FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            var listMenu = new List<IdCodeNameSelected>();
            var allMenu = _repositoryMenu.Get().Where(x => x.IsApproved).ToList();

            var itemMenus = _repositoryPermissionMenuLine
                .Join(x => x.Permission).Where(x => x.Permission.Id == item.Id).Select(x => x.Menu.Id);

            foreach (var itemMenu in allMenu)
            {
                listMenu.Add(itemMenus.Contains(itemMenu.Id) ? new IdCodeNameSelected(itemMenu.Id, itemMenu.Code
                ,itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", true) : new IdCodeNameSelected(itemMenu.Id, itemMenu.Code, itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", false));
            }



            var modelItem = item.CreateMapped<Permission, PermissionModel>();
            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Menus = listMenu;

            return new DetailModel<PermissionModel>
            {
                Item = modelItem
            };
        }

        public AddModel<PermissionModel> Add()
        {
            return new AddModel<PermissionModel>();
        }

        public AddModel<PermissionModel> Add(AddModel<PermissionModel> addModel)
        {
            IValidator validator = new FluentValidator<PermissionModel, PermissionValidationRules>(addModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<PermissionModel, Permission>();

            if (_repositoryPermission.Get().FirstOrDefault(e => e.Code == item.Code) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
            }
            item.Id = GuidHelper.NewGuid();
            item.Version = 1;
            item.CreationTime = DateTime.Now;
            item.LastModificationTime = DateTime.Now;
            item.DisplayOrder = 1;

            item.Creator = IdentityUser ?? throw new IdentityUserException(Messages.DangerIdentityUserNotFound);
            item.LastModifier = IdentityUser;
            _repositoryPermission.Add(item, true);
            var maxDisplayOrder = _repositoryPermission.Get().Max(e => e.DisplayOrder);
            item.DisplayOrder = maxDisplayOrder + 1;
            var affectedItem = _repositoryPermission.Update(item, true);
            var itemHistory = affectedItem.CreateMapped<Permission, PermissionHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryPermissionHistory.Add(itemHistory, true);

            addModel.Item = affectedItem.CreateMapped<Permission, PermissionModel>();

            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);

            return addModel;
        }

        public UpdateModel<PermissionModel> Update(Guid id)
        {
            var item = _repositoryPermission.Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person).FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }
            var listMenu = new List<IdCodeNameSelected>();
            var allMenu = _repositoryMenu.Get().Where(x => x.IsApproved).ToList();

            var itemMenus = _repositoryPermissionMenuLine
                .Join(x => x.Permission)
                .Where(x => x.Permission.Id == item.Id).Select(x => x.Menu.Id);

            foreach (var itemMenu in allMenu)
            {
                listMenu.Add(itemMenus.Contains(itemMenu.Id) ? new IdCodeNameSelected(itemMenu.Id, itemMenu.Code, itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", true) : new IdCodeNameSelected(itemMenu.Id, itemMenu.Code, itemMenu.Name + " (Kod: " + itemMenu.Code + " | Adres: " + itemMenu.Address + ")", false));
            }

            var modelItem = item.CreateMapped<Permission, PermissionModel>();
            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.Menus = listMenu;
            return new UpdateModel<PermissionModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<PermissionModel> Update(UpdateModel<PermissionModel> updateModel)
        {
            IValidator validator = new FluentValidator<PermissionModel, PermissionValidationRules>(updateModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryPermission

                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
               .FirstOrDefault(e => e.Id == updateModel.Item.Id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryPermission.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var versionHistoryPermission = _repositoryPermissionHistory.Get().Where(e => e.ReferenceId == item.Id).Max(t => t.Version);

            var itemHistory = item.CreateMapped<Permission, PermissionHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.IsDeleted = false;
            itemHistory.Version = versionHistoryPermission + 1;
            itemHistory.RestoreVersion = 0;
            _repositoryPermissionHistory.Add(itemHistory, true);

            item.Code = updateModel.Item.Code;
            item.Name = updateModel.Item.Name;
            item.Description = updateModel.Item.Description;
            item.ControllerName = updateModel.Item.ControllerName;
            item.ActionName = updateModel.Item.ActionName;

            item.IsApproved = updateModel.Item.IsApproved;
            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;

            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            var version = item.Version;
            item.Version = version + 1;
            var affectedItem = _repositoryPermission.Update(item, true);

            foreach (var line in _repositoryPermissionMenuLine
                .Join(x => x.Menu)
                .Join(x => x.Permission)
                .Where(x => x.Permission.Id == affectedItem.Id).ToList())
            {
                var lineHistory = line.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.MenuId = line.Menu.Id;
                lineHistory.PermissionId = line.Permission.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = IdentityUser.Id;
                _repositoryPermissionMenuLineHistory.Add(lineHistory, true);
                _repositoryPermissionMenuLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Menus)
            {
                var itemMenu = _repositoryMenu.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryPermissionMenuLine.Add(new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Menu = itemMenu,
                    Permission = affectedItem,
                    Creator = IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.MenuId = affectedLine.Menu.Id;
                lineHistory.PermissionId = affectedLine.Permission.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryPermissionMenuLineHistory.Add(lineHistory, true);
            }

            updateModel.Item = affectedItem.CreateMapped<Permission, PermissionModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            updateModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);


            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryRolePermissionLine.Get().Count(x => x.Permission.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            if (_repositoryPermissionMenuLine.Get().Count(x => x.Permission.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }


            var version = _repositoryPermissionHistory.Get().Where(e => e.ReferenceId == id).Max(t => t.Version);
            var item = _repositoryPermission.Get(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var itemHistory = item.CreateMapped<Permission, PermissionHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.Version = version + 1;
            itemHistory.IsDeleted = true;

            _repositoryPermissionHistory.Add(itemHistory, true);
            _repositoryPermission.Delete(item, true);
        }

    }
}