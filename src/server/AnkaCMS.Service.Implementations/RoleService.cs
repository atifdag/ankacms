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
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _repositoryRole;
        private readonly IRepository<RoleHistory> _repositoryRoleHistory;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<RoleUserLine> _repositoryRoleUserLine;
        private readonly IRepository<Permission> _repositoryPermission;
        private readonly IRepository<RolePermissionLineHistory> _repositoryRolePermissionLineHistory;
        private readonly IMainService _serviceMain;

        public RoleService(IRepository<Role> repositoryRole, IRepository<RoleHistory> repositoryRoleHistory, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<User> repositoryUser, IRepository<RoleUserLine> repositoryRoleUserLine, IRepository<Permission> repositoryPermission, IRepository<RolePermissionLineHistory> repositoryRolePermissionLineHistory, IMainService serviceMain)
        {
            _repositoryRole = repositoryRole;
            _repositoryRoleHistory = repositoryRoleHistory;
            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryUser = repositoryUser;

            _repositoryRoleUserLine = repositoryRoleUserLine;
            _repositoryPermission = repositoryPermission;
            _repositoryRolePermissionLineHistory = repositoryRolePermissionLineHistory;
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
                    .Join(x => x.RoleUserLines)
                    .ThenJoin(x => x.Role)
                    .FirstOrDefault(a => a.Id == identity.UserId);





                // Kullanıcı bulunamadı ise
                if (user == null)
                {
                    throw new NotFoundException(Messages.DangerIdentityUserNotFound);
                }

                return user;
            }
        }
        public ListModel<RoleModel> List(FilterModel filterModel)
        {
            var startDate = filterModel.StartDate.ResetTimeToStartOfDay();
            var endDate = filterModel.EndDate.ResetTimeToEndOfDay();
            Expression<Func<Role, bool>> expression;

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

            var model = filterModel.CreateMapped<FilterModel, ListModel<RoleModel>>();

            if (model.Paging == null)
            {
                model.Paging = new Paging
                {
                    PageSize = filterModel.PageSize,
                    PageNumber = filterModel.PageNumber
                };
            }

            var sortHelper = new SortHelper<RoleModel>();

            var query = (IOrderedQueryable<Role>)_repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Where(expression);

            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<RoleModel>();
            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<Role, RoleModel>();
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

        public DetailModel<RoleModel> Detail(Guid id)
        {
            var item = _repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.RolePermissionLines)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var listPermission = new List<IdCodeNameSelected>();
            var allPermission = _repositoryPermission.Get().Where(x => x.IsApproved).ToList();

            var itemPermissions = _repositoryRolePermissionLine
                .Join(x => x.Permission)
                .Where(x => x.Role.Id == item.Id).Select(x => x.Permission.Id);

            foreach (var itemPermission in allPermission)
            {
                listPermission.Add(itemPermissions.Contains(itemPermission.Id) ? new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, true) : new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, false));
            }

            var modelItem = item.CreateMapped<Role, RoleModel>();
            modelItem.Permissions = listPermission;

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            return new DetailModel<RoleModel>
            {
                Item = modelItem
            };
        }

        public AddModel<RoleModel> Add()
        {
            return new AddModel<RoleModel>();
        }

        public AddModel<RoleModel> Add(AddModel<RoleModel> addModel)
        {
            IValidator validator = new FluentValidator<RoleModel, RoleValidationRules>(addModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<RoleModel, Role>();

            if (_repositoryRole.Get().FirstOrDefault(e => e.Code == item.Code) != null)
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
            _repositoryRole.Add(item, true);
            var maxDisplayOrder = _repositoryRole.Get().Max(e => e.DisplayOrder);
            item.DisplayOrder = maxDisplayOrder + 1;
            var affectedItem = _repositoryRole.Update(item, true);
            var itemHistory = affectedItem.CreateMapped<Role, RoleHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = affectedItem.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            _repositoryRoleHistory.Add(itemHistory, true);

            addModel.Item = affectedItem.CreateMapped<Role, RoleModel>();

            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);

            return addModel;
        }

        public UpdateModel<RoleModel> Update(Guid id)
        {
            var item = _repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.RolePermissionLines)
                .FirstOrDefault(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var listPermission = new List<IdCodeNameSelected>();
            var allPermission = _repositoryPermission.Get().Where(x => x.IsApproved).ToList();

            var itemPermissions = _repositoryRolePermissionLine
                .Join(x => x.Permission)
                .Where(x => x.Role.Id == item.Id).Select(x => x.Permission.Id);

            foreach (var itemPermission in allPermission)
            {
                listPermission.Add(itemPermissions.Contains(itemPermission.Id) ? new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, true) : new IdCodeNameSelected(itemPermission.Id, itemPermission.Code, itemPermission.Name, false));
            }

            var modelItem = item.CreateMapped<Role, RoleModel>();
            modelItem.Permissions = listPermission;

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            return new UpdateModel<RoleModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<RoleModel> Update(UpdateModel<RoleModel> updateModel)
        {
            IValidator validator = new FluentValidator<RoleModel, RoleValidationRules>(updateModel.Item);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = _repositoryRole
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .FirstOrDefault(e => e.Id == updateModel.Item.Id);

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (updateModel.Item.Code != item.Code)
            {
                if (_repositoryRole.Get().Any(p => p.Code == updateModel.Item.Code))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Code));
                }
            }

            var versionHistoryRole = _repositoryRoleHistory.Get().Where(e => e.ReferenceId == item.Id).Max(t => t.Version);

            var itemHistory = item.CreateMapped<Role, RoleHistory>();
            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.IsDeleted = false;
            itemHistory.Version = versionHistoryRole + 1;
            itemHistory.RestoreVersion = 0;
            _repositoryRoleHistory.Add(itemHistory, true);

            item.Code = updateModel.Item.Code;
            item.Name = updateModel.Item.Name;
            item.Description = updateModel.Item.Description;
            item.Level = updateModel.Item.Level;
            item.IsApproved = updateModel.Item.IsApproved;
            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;

            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            var version = item.Version;
            item.Version = version + 1;
            var affectedItem = _repositoryRole.Update(item, true);



            foreach (var line in _repositoryRolePermissionLine
                .Join(x => x.Role)
                .Join(x => x.Permission)
                .Where(x => x.Role.Id == affectedItem.Id).ToList())
            {
                var lineHistory = line.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.RoleId = line.Role.Id;
                lineHistory.PermissionId = line.Permission.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = IdentityUser.Id;
                _repositoryRolePermissionLineHistory.Add(lineHistory, true);
                _repositoryRolePermissionLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Permissions)
            {
                var itemPermission = _repositoryPermission.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryRolePermissionLine.Add(new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Role = affectedItem,
                    Creator = IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.RoleId = affectedLine.Role.Id;
                lineHistory.PermissionId = affectedLine.Permission.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryRolePermissionLineHistory.Add(lineHistory, true);
            }




            updateModel.Item = affectedItem.CreateMapped<Role, RoleModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            updateModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);

            return updateModel;

        }

        public void Delete(Guid id)
        {
            if (_repositoryRolePermissionLine.Get().Count(x => x.Role.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }
            if (_repositoryRoleUserLine.Get().Count(x => x.Role.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }


            var version = _repositoryRoleHistory.Get().Where(e => e.ReferenceId == id).Max(t => t.Version);
            var item = _repositoryRole.Get(x => x.Id == id);
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var itemHistory = item.CreateMapped<Role, RoleHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.Version = version + 1;
            itemHistory.IsDeleted = true;

            _repositoryRoleHistory.Add(itemHistory, true);
            _repositoryRole.Delete(item, true);
        }

        public List<Guid> GetActionRoles(string controller, string action)
        {
            return _repositoryRolePermissionLine
                .Join(x => x.Permission)
                .Join(x => x.Role)
                .Where(x => x.Permission.ControllerName == controller && x.Permission.ActionName == action)
                .Select(permissionRoleLine => permissionRoleLine.Role).Select(role => role.Id).ToList();
        }

        public List<IdCodeName> List()
        {
            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();

            var list = _repositoryRole.Get().Where(x => x.IsApproved && x.Level > identityUserMinRoleLevel).OrderBy(x => x.DisplayOrder).Select(x => new IdCodeName(x.Id,x.Code, x.Name));
            if (list.Any())
            {
                return list.ToList();
            }
            throw new NotFoundException(Messages.DangerRecordNotFound);
        }

    }
}
