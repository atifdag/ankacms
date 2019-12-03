using AnkaCMS.Data.DataAccess.EntityFramework;
using AnkaCMS.Data.DataEntities;
using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Service.Implementations.EmailMessaging;
using AnkaCMS.Core.Enums;
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
    public class UserService : IUserService
    {

        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<RolePermissionLine> _repositoryRolePermissionLine;
        private readonly IRepository<PermissionMenuLine> _repositoryPermissionMenuLine;
        private readonly IRepository<RoleUserLine> _repositoryRoleUserLine;
        private readonly IRepository<UserHistory> _repositoryUserHistory;
        private readonly IRepository<Person> _repositoryPerson;
        private readonly IRepository<PersonHistory> _repositoryPersonHistory;
        private readonly IRepository<Role> _repositoryRole;
        private readonly IRepository<RoleUserLineHistory> _repositoryRoleUserLineHistory;


        private readonly ISmtp _smtp;
        private readonly IMainService _serviceMain;

        public UserService(IRepository<User> repositoryUser, IRepository<RolePermissionLine> repositoryRolePermissionLine, IRepository<PermissionMenuLine> repositoryPermissionMenuLine, IRepository<RoleUserLine> repositoryRoleUserLine, IRepository<UserHistory> repositoryUserHistory, ISmtp smtp, IRepository<Person> repositoryPerson, IRepository<PersonHistory> repositoryPersonHistory, IRepository<Role> repositoryRole, IRepository<RoleUserLineHistory> repositoryRoleUserLineHistory, IMainService serviceMain)
        {
            _repositoryUser = repositoryUser;
            _repositoryRolePermissionLine = repositoryRolePermissionLine;
            _repositoryPermissionMenuLine = repositoryPermissionMenuLine;
            _repositoryRoleUserLine = repositoryRoleUserLine;
            _repositoryUserHistory = repositoryUserHistory;
            _smtp = smtp;
            _repositoryPerson = repositoryPerson;
            _repositoryPersonHistory = repositoryPersonHistory;
            _repositoryRole = repositoryRole;
            _repositoryRoleUserLineHistory = repositoryRoleUserLineHistory;
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
                    .Join(x=>x.RoleUserLines)
                    .ThenJoin(x=>x.Role)
                    .FirstOrDefault(a => a.Id == identity.UserId);




                // Kullanıcı bulunamadı ise
                if (user == null)
                {
                    throw new NotFoundException(Messages.DangerIdentityUserNotFound);
                }

                return user;
            }
        }

        private ListModel<UserModel> List(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, int status, string searched, List<Guid> parentIds, ListModel<UserModel> model)
        {
            var resetedStartDate = startDate.ResetTimeToStartOfDay();
            var resetedEndDate = endDate.ResetTimeToEndOfDay();

            Expression<Func<User, bool>> expression;

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
                        expression = c => c.IsApproved && c.Username.Contains(searched);
                    }
                    else
                    {
                        expression = c => c.IsApproved == false && c.Username.Contains(searched);
                    }
                }
                else
                {
                    if (bStatus)
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
                if (searched != null)
                {
                    expression = c => c.Username.Contains(searched);
                }
                else
                {
                    expression = c => c.Id != Guid.Empty;
                }
            }

            expression = expression.And(e => e.CreationTime >= resetedStartDate && e.CreationTime <= resetedEndDate);

            if (parentIds != null)
            {
                if (parentIds.Count > 0)
                {
                    expression = expression.And(e => e.RoleUserLines.Any(x => parentIds.Contains(x.Role.Id)));
                }
            }

            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x=>x.Role.Level).Min();

            expression = expression.And(x => x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));


            var sortHelper = new SortHelper<UserModel>();

           


            var query = (IOrderedQueryable<User>)_repositoryUser
                .Join(x=>x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x=>x.RoleUserLines)
                .ThenJoin(x=>x.Role)
                .Where(expression);

            


            sortHelper.OrderBy(x => x.DisplayOrder);

            model.Paging.TotalItemCount = query.Count();
            var items = model.Paging.PageSize > 0 ? query.Skip((model.Paging.PageNumber - 1) * model.Paging.PageSize).Take(model.Paging.PageSize) : query;
            var modelItems = new HashSet<UserModel>();

            // var modelItemRoles = new List<KeyValueSelected>();

            foreach (var item in items)
            {
                var modelItem = item.CreateMapped<User, UserModel>();
                modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
                modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
                modelItem.BirthDate = item.Person.BirthDate;
                modelItem.IdentityCode = item.Person.IdentityCode;
                modelItem.FirstName = item.Person.FirstName;
                modelItem.LastName = item.Person.LastName;
                modelItem.Biography = item.Person.Biography;
                modelItem.Roles = item.RoleUserLines.Select(t => t.Role).Select(role => new IdCodeNameSelected(role.Id, role.Code, role.Name, true)).ToList();
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

        public ListModel<UserModel> List(FilterModel filterModel)
        {
            var model = filterModel.CreateMapped<FilterModel, ListModel<UserModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, null, model);
        }

        public DetailModel<UserModel> Detail(Guid id)
        {
            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();


            var item = _repositoryUser

                  .Join(x => x.Person)
                  .Join(x => x.Creator.Person)
                  .Join(x => x.LastModifier.Person)
                  .Join(x => x.RoleUserLines)
                    .FirstOrDefault(x => x.Id == id && x.RoleUserLines.All(t=>t.Role.Level>identityUserMinRoleLevel));


            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var roleList = new List<IdCodeNameSelected>();

            var allRoles = _repositoryRole.Get().Where(x => x.IsApproved && x.Level> identityUserMinRoleLevel).ToList();
            var userRoles = _repositoryRoleUserLine
                .Join(x => x.Role)
                .Where(x => x.User.Id == item.Id).Select(x => x.Role.Id).ToList();

            foreach (var role in allRoles)
            {
                roleList.Add(userRoles.Contains(role.Id) ? new IdCodeNameSelected(role.Id, role.Code, role.Name, true) : new IdCodeNameSelected(role.Id, role.Code, role.Name, false));
            }

            var modelItem = item.CreateMapped<User, UserModel>();
            modelItem.Roles = roleList;
            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);
            modelItem.BirthDate = item.Person.BirthDate;
            modelItem.IdentityCode = item.Person.IdentityCode;
            modelItem.FirstName = item.Person.FirstName;
            modelItem.LastName = item.Person.LastName;
            modelItem.Biography = item.Person.Biography;
            return new DetailModel<UserModel>
            {
                Item = modelItem
            };
        }

        public AddModel<UserModel> Add()
        {
            return new AddModel<UserModel>();
        }

        public AddModel<UserModel> Add(AddModel<UserModel> addModel)
        {
            IValidator validator = new FluentValidator<UserModel, UserValidationRules>(addModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var item = addModel.Item.CreateMapped<UserModel, User>();

            if (_repositoryUser.Get(e => e.Username == item.Username) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
            }

            if (_repositoryUser.Get(e => e.Email == item.Email) != null)
            {
                throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
            }

            if (addModel.Item.BirthDate == DateTime.MinValue)
            {
                addModel.Item.BirthDate = DateTime.Now;
            }

            var password = addModel.Item.Password.ToSha512();

            // kişi bilgisi veritabanında var mı?
            // Kişi bilgisi yoksa yeni kişi bilgisi oluşturuluyor
            Person person;

            var maxDisplayOrderPerson = _repositoryPerson.Get().Max(e => e.DisplayOrder);
            var maxDisplayOrderUser = _repositoryUser.Get().Max(e => e.DisplayOrder);

            if (addModel.Item.IdentityCode != null)
            {
                if (_repositoryPerson.Get(x => x.IdentityCode == addModel.Item.IdentityCode) != null)
                {
                    person = _repositoryPerson.Get(x => x.IdentityCode == addModel.Item.IdentityCode);
                }
                else
                {
                    person = new Person
                    {
                        Id = GuidHelper.NewGuid(),
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        DisplayOrder = maxDisplayOrderPerson + 1,
                        Version = 1,
                        IsApproved = false,
                        IdentityCode = addModel.Item.IdentityCode.Trim().Length > 0 ? addModel.Item.IdentityCode : GuidHelper.NewGuid().ToString(),
                        FirstName = addModel.Item.FirstName,
                        LastName = addModel.Item.LastName,
                        Biography = addModel.Item.Biography,
                        BirthDate = addModel.Item.BirthDate
                    };
                }
            }

            else
            {
                person = new Person
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = maxDisplayOrderPerson + 1,
                    Version = 1,
                    IsApproved = false,
                    IdentityCode = GuidHelper.NewGuid().ToString(),
                    FirstName = addModel.Item.FirstName,
                    LastName = addModel.Item.LastName,
                    BirthDate = addModel.Item.BirthDate,
                    Biography = addModel.Item.Biography

                };
            }




            person.CreatorId = IdentityUser.Id;
            person.LastModifierId = IdentityUser.Id;

            _repositoryPerson.Add(person, true);
            item.Id = GuidHelper.NewGuid();
            item.Creator = IdentityUser;
            item.CreationTime = DateTime.Now;

            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            item.DisplayOrder = maxDisplayOrderUser + 1;
            item.Person = person;
            item.Version = 1;

            var affectedUser = _repositoryUser.Add(item, true);

            var personHistory = person.CreateMapped<Person, PersonHistory>();
            personHistory.Id = GuidHelper.NewGuid();
            personHistory.ReferenceId = person.Id;
            personHistory.IsDeleted = false;
            personHistory.RestoreVersion = 0;
            _repositoryPersonHistory.Add(personHistory, true);

            var userHistory = affectedUser.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = affectedUser.Id;
            userHistory.PersonId = affectedUser.Person.Id;
            userHistory.CreatorId = IdentityUser.Id;
            userHistory.IsDeleted = false;
            userHistory.RestoreVersion = 0;

            _repositoryUserHistory.Add(userHistory, true);

            var role = _repositoryRole.Get(x => x.Code == "DEFAULTROLE");

            if (role == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var affectedRoleUserLine = _repositoryRoleUserLine.Add(new RoleUserLine
            {
                Id = GuidHelper.NewGuid(),
                User = affectedUser,
                Role = role,
                Creator = IdentityUser,
                CreationTime = DateTime.Now,
                DisplayOrder = 1,
                LastModifier = IdentityUser,
                LastModificationTime = DateTime.Now,
                Version = 1

            }, true);

            var roleUserLineHistory = affectedRoleUserLine.CreateMapped<RoleUserLine, RoleUserLineHistory>();
            roleUserLineHistory.Id = GuidHelper.NewGuid();
            roleUserLineHistory.ReferenceId = roleUserLineHistory.Id;
            roleUserLineHistory.RoleId = affectedRoleUserLine.Role.Id;
            roleUserLineHistory.UserId = affectedRoleUserLine.User.Id;
            roleUserLineHistory.CreatorId = affectedRoleUserLine.Creator.Id;

            _repositoryRoleUserLineHistory.Add(roleUserLineHistory, true);

            addModel.Item.Creator = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);
            addModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);


            addModel.Item.BirthDate = affectedUser.Person.BirthDate;
            addModel.Item.IdentityCode = affectedUser.Person.IdentityCode;
            addModel.Item.FirstName = affectedUser.Person.FirstName;
            addModel.Item.LastName = affectedUser.Person.LastName;
            addModel.Item.Biography = affectedUser.Person.Biography;

            if (!_serviceMain.ApplicationSettings.SendMailAfterAddUser) return addModel;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = password,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_smtp, _serviceMain);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.Add);
            return addModel;

           
        }

        public UpdateModel<UserModel> Update(Guid id)
        {
            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();

            var item = _repositoryUser
                .Join(x => x.Person)
                  .Join(x => x.Creator.Person)
                  .Join(x => x.LastModifier.Person)
                  .Join(x => x.RoleUserLines)
                .FirstOrDefault(x => x.Id == id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));

            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var roleList = new List<IdCodeNameSelected>();

            var allRoles = _repositoryRole.Get().Where(x => x.IsApproved && x.Level > identityUserMinRoleLevel).ToList();
            var userRoles = _repositoryRoleUserLine
                .Join(x => x.Role)
                .Where(x => x.User.Id == item.Id).Select(x => x.Role.Id).ToList();

            foreach (var role in allRoles)
            {
                roleList.Add(userRoles.Contains(role.Id) ? new IdCodeNameSelected(role.Id, role.Code, role.Name, true) : new IdCodeNameSelected(role.Id, role.Code, role.Name, false));
            }


            var modelItem = item.CreateMapped<User, UserModel>();
            modelItem.Roles = roleList;

            modelItem.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            modelItem.LastModifier = new IdCodeName(item.LastModifier.Id,item.LastModifier.Username, item.LastModifier.Person.DisplayName);

            modelItem.BirthDate = item.Person.BirthDate;
            modelItem.IdentityCode = item.Person.IdentityCode;
            modelItem.FirstName = item.Person.FirstName;
            modelItem.LastName = item.Person.LastName;
            modelItem.Biography = item.Person.Biography;

            return new UpdateModel<UserModel>
            {
                Item = modelItem
            };
        }

        public UpdateModel<UserModel> Update(UpdateModel<UserModel> updateModel)
        {
            IValidator validator = new FluentValidator<UserModel, UserValidationRules>(updateModel.Item);
            var validationResults = validator.Validate();
            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();


            var item = _repositoryUser
                .Join(x => x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .FirstOrDefault(x => x.Id == updateModel.Item.Id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var person = item.Person;

            if (updateModel.Item.Username != item.Username)
            {
                if (_repositoryUser.Get().Any(p => p.Username == updateModel.Item.Username))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
                }
            }

            if (updateModel.Item.Email != item.Email)
            {
                if (_repositoryUser.Get().Any(p => p.Email == updateModel.Item.Email))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
                }
            }


            var versionHistoryPerson = _repositoryPersonHistory.Get().Where(e => e.ReferenceId == person.Id).Max(t => t.Version);

            var personHistory = person.CreateMapped<Person, PersonHistory>();
            personHistory.Id = GuidHelper.NewGuid();
            personHistory.ReferenceId = item.Id;
            personHistory.CreatorId = IdentityUser.Id;
            personHistory.CreationTime = DateTime.Now;
            personHistory.IsDeleted = false;
            personHistory.Version = versionHistoryPerson + 1;
            personHistory.RestoreVersion = 0;
            _repositoryPersonHistory.Add(personHistory, true);

            person.FirstName = updateModel.Item.FirstName;
            person.LastName = updateModel.Item.LastName;
            person.Biography = updateModel.Item.Biography;








            person.LastModificationTime = DateTime.Now;
            person.LastModifierId = IdentityUser.Id;
            var versionPerson = person.Version;
            person.Version = versionPerson + 1;
            _repositoryPerson.Update(person, true);

            var versionHistoryUser = _repositoryUserHistory.Get().Where(e => e.ReferenceId == item.Id).Max(t => t.Version);

            var userHistory = item.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = item.Id;
            userHistory.CreatorId = IdentityUser.Id;
            userHistory.CreationTime = DateTime.Now;
            userHistory.IsDeleted = false;
            userHistory.Version = versionHistoryUser + 1;
            userHistory.RestoreVersion = 0;
            _repositoryUserHistory.Add(userHistory, true);

            item.Username = updateModel.Item.Username;
            item.Email = updateModel.Item.Email;
            item.IsApproved = updateModel.Item.IsApproved;

            item.LastModificationTime = DateTime.Now;
            item.LastModifier = IdentityUser;
            var version = item.Version;
            item.Version = version + 1;
            var affectedUser = _repositoryUser.Update(item, true);

            foreach (var line in _repositoryRoleUserLine

                .Join(x => x.Role)
                .Join(x => x.User)
                .Where(x => x.User.Id == affectedUser.Id).ToList())
            {
                var lineHistory = line.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.RoleId = line.Role.Id;
                lineHistory.UserId = line.User.Id;
                lineHistory.CreationTime = DateTime.Now;
                lineHistory.CreatorId = IdentityUser.Id;
                _repositoryRoleUserLineHistory.Add(lineHistory, true);
                _repositoryRoleUserLine.Delete(line, true);
            }

            foreach (var idCodeNameSelected in updateModel.Item.Roles)
            {
                var itemRole = _repositoryRole.Get(x => x.Id == idCodeNameSelected.Id);

                var affectedLine = _repositoryRoleUserLine.Add(new RoleUserLine
                {
                    Id = GuidHelper.NewGuid(),
                    User = affectedUser,
                    Role = itemRole,
                    Creator = IdentityUser,
                    CreationTime = DateTime.Now,
                    DisplayOrder = 1,
                    LastModifier = IdentityUser,
                    LastModificationTime = DateTime.Now,
                    Version = 1

                }, true);

                var lineHistory = affectedLine.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = affectedLine.Id;
                lineHistory.RoleId = affectedLine.Role.Id;
                lineHistory.UserId = affectedLine.User.Id;
                lineHistory.CreatorId = affectedLine.Creator.Id;

                _repositoryRoleUserLineHistory.Add(lineHistory, true);
            }

            updateModel.Item = affectedUser.CreateMapped<User, UserModel>();

            updateModel.Item.Creator = new IdCodeName(item.Creator.Id, item.Creator.Username, item.Creator.Person.DisplayName);
            updateModel.Item.LastModifier = new IdCodeName(IdentityUser.Id,IdentityUser.Username, IdentityUser.Person.DisplayName);


            updateModel.Item.BirthDate = item.Person.BirthDate;
            updateModel.Item.IdentityCode = item.Person.IdentityCode;
            updateModel.Item.FirstName = item.Person.FirstName;
            updateModel.Item.LastName = item.Person.LastName;
            updateModel.Item.Biography = item.Person.Biography;



            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserInformation) return updateModel;
            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = string.Empty,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_smtp, _serviceMain);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.Update);
            return updateModel;



        }

        public void Delete(Guid id)
        {
            if (_repositoryRoleUserLine.Get().Count(x => x.User.Id == id) > 0)
            {
                throw new InvalidTransactionException(Messages.DangerAssociatedRecordNotDeleted);
            }

            var identityUserMinRoleLevel = IdentityUser.RoleUserLines.Select(x => x.Role.Level).Min();


            var version = _repositoryUserHistory.Get().Where(e => e.ReferenceId == id).Max(t => t.Version);
            var item = _repositoryUser.Get(x => x.Id == id && x.RoleUserLines.All(t => t.Role.Level > identityUserMinRoleLevel));
            if (item == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            var itemHistory = item.CreateMapped<User, UserHistory>();

            itemHistory.Id = GuidHelper.NewGuid();
            itemHistory.ReferenceId = item.Id;
            itemHistory.CreationTime = DateTime.Now;

            itemHistory.CreatorId = IdentityUser.Id;
            itemHistory.Version = version + 1;
            itemHistory.IsDeleted = true;

            _repositoryUserHistory.Add(itemHistory, true);
            _repositoryUser.Delete(item, true);
        }


        public ListModel<UserModel> List(FilterModelWithMultiParent filterModel)
        {
            var model = filterModel.CreateMapped<FilterModelWithMultiParent, ListModel<UserModel>>();
            return List(filterModel.StartDate, filterModel.EndDate, filterModel.PageNumber, filterModel.PageSize, filterModel.Status, filterModel.Searched, filterModel.Parents.Select(t=>t.Id).ToList(), model);
        }

        public MyProfileModel MyProfile()
        {
            var lastLoginTime = DateTime.Now;
            MyProfileModel myProfileModel;
            UserModel userModel;
            var identity = (AnkaCMSIdentity)Thread.CurrentPrincipal.Identity;
            var cacheKeyProfile = CacheKeyOption.Profile + "-" + identity.UserId;
            var identityUser = _repositoryUser
                .Join(x => x.Person)
                .Join(x => x.Creator.Person)
                .Join(x => x.LastModifier.Person)
                .Join(x => x.SessionsCreatedBy)
                .Join(x => x.SessionHistoriesCreatedBy)
                .FirstOrDefault(e => e.Id == identity.UserId);

            if (identityUser == null) throw new NotFoundException(Messages.DangerRecordNotFound);

            var menuList = new List<Menu>();

            var roleUserLines = _repositoryRoleUserLine

                .Join(x => x.Role)
                .Join(x => x.Role.RolePermissionLines)
                .Where(x => x.User == identityUser && x.Role.IsApproved).ToList();

            foreach (var roleUserLine in roleUserLines)
            {
                var role = roleUserLine.Role;

                var rolePermissionLines = _repositoryRolePermissionLine

                    .Join(x => x.Permission.PermissionMenuLines)
                    .Where(x => x.Role == role && x.Permission.IsApproved).OrderBy(x => x.Permission.DisplayOrder).ToList();

                foreach (var rolePermissionLine in rolePermissionLines)
                {
                    var permission = rolePermissionLine.Permission;

                    var permissionMenuLines = _repositoryPermissionMenuLine

                          .Join(x => x.Menu.ParentMenu)
                          .Join(x => x.Menu.ChildMenus)
                        .Where(x => x.Permission == permission && x.Menu.IsApproved).OrderBy(x => x.Menu.DisplayOrder).ToList();


                    foreach (var permissionPermissionMenuLine in permissionMenuLines)
                    {
                        var menu = permissionPermissionMenuLine.Menu;
                        if (menuList.FirstOrDefault(x => x.Id == menu.Id) == null)
                        {
                            menuList.Add(menu);
                        }
                    }
                }

            }

            var rootMenus = new List<RootMenu>();
            foreach (var menuEntity in menuList.OrderBy(x => x.Code))
            {
                if (menuEntity.ParentMenu.Code != "ROOTMENU") continue;
                var rootMenu = menuEntity.CreateMapped<Menu, RootMenu>();
                if (menuEntity.ChildMenus.Any())
                {
                    rootMenu.ChildMenus = new List<ChildMenu>();
                    foreach (var childMenuEntity in menuEntity.ChildMenus)
                    {
                        var childMenu = childMenuEntity.CreateMapped<Menu, ChildMenu>();

                        if (childMenuEntity.ChildMenus != null)
                        {

                            if (childMenuEntity.ChildMenus.Any())
                            {
                                childMenu.LeafMenus = new List<LeafMenu>();

                                foreach (var leafMenuEntity in childMenuEntity.ChildMenus)
                                {
                                    var leafMenu = leafMenuEntity.CreateMapped<Menu, LeafMenu>();
                                    leafMenu.Parent = childMenu;
                                    childMenu.LeafMenus.Add(leafMenu);
                                }


                            }
                        }

                        rootMenu.ChildMenus.Add(childMenu);
                    }
                }

                rootMenus.Add(rootMenu);
            }




            var sessionHistories = identityUser.SessionHistoriesCreatedBy;
            if (!(sessionHistories?.Count > 0))
            {

                userModel = identityUser.CreateMapped<User, UserModel>();
                userModel.Creator = new IdCodeName(identityUser.Creator.Id, identityUser.Creator.Username, identityUser.Creator.Person.DisplayName);
                userModel.LastModifier = new IdCodeName(identityUser.LastModifier.Id, identityUser.LastModifier.Username, identityUser.LastModifier.Person.DisplayName);

                userModel.BirthDate = identityUser.Person.BirthDate;
                userModel.IdentityCode = identityUser.Person.IdentityCode;
                userModel.FirstName = identityUser.Person.FirstName;
                userModel.LastName = identityUser.Person.LastName;
                userModel.Biography = identityUser.Person.Biography;


                userModel.Password = null;
                userModel.Roles = roleUserLines.Select(t => new IdCodeNameSelected(t.Role.Id, t.Role.Code, t.Role.Name, true)).ToList();
                myProfileModel = new MyProfileModel
                {
                    UserModel = userModel,
                    LastLoginTime = lastLoginTime,
                    RootMenus = rootMenus
                };

                return myProfileModel;

            }

            var lastSession = sessionHistories.OrderByDescending(e => e.LastModificationTime).FirstOrDefault();
            if (lastSession != null)
            {
                lastLoginTime = lastSession.LastModificationTime;
            }

            userModel = identityUser.CreateMapped<User, UserModel>();
            userModel.Creator = new IdCodeName(identityUser.Creator.Id, identityUser.Creator.Username, identityUser.Creator.Person.DisplayName);
            userModel.LastModifier = new IdCodeName(identityUser.LastModifier.Id, identityUser.LastModifier.Username, identityUser.LastModifier.Person.DisplayName);


            userModel.BirthDate = identityUser.Person.BirthDate;
            userModel.IdentityCode = identityUser.Person.IdentityCode;
            userModel.FirstName = identityUser.Person.FirstName;
            userModel.LastName = identityUser.Person.LastName;
            userModel.Biography = identityUser.Person.Biography;

            userModel.Password = null;
            userModel.Roles = roleUserLines.Select(t => new IdCodeNameSelected(t.Role.Id, t.Role.Code, t.Role.Name, true)).ToList();

            myProfileModel = new MyProfileModel
            {
                UserModel = userModel,
                LastLoginTime = lastLoginTime,
                RootMenus = rootMenus
            };

            return myProfileModel;

        }

        public void UpdateMyPassword(UpdatePasswordModel model)
        {
            IValidator validator = new FluentValidator<UpdatePasswordModel, UpdatePasswordModelValidationRules>(model);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var identity = (AnkaCMSIdentity)Thread.CurrentPrincipal.Identity;
            var user = _repositoryUser.Get(e => e.Id == identity.UserId);

            if (user == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }

            if (model.OldPassword.ToSha512() != user.Password)
            {
                throw new NotFoundException(Messages.DangerIncorrectOldPassword);
            }

            var versionHistory = _repositoryUserHistory.Get().Where(e => e.ReferenceId == user.Id).Max(t => t.Version);

            var userHistory = user.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = user.Id;
            userHistory.CreatorId = user.Creator.Id;
            userHistory.IsDeleted = false;
            userHistory.Version = versionHistory + 1;
            userHistory.RestoreVersion = 0;
            _repositoryUserHistory.Add(userHistory, true);


            var password = model.Password;
            user.Password = password.ToSha512();
            user.LastModificationTime = DateTime.Now;
            user.LastModifier = user;
            var version = user.Version;
            user.Version = version + 1;
            var affectedUser = _repositoryUser.Update(user, true);
            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserPassword) return;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = password,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_smtp, _serviceMain);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.UpdateMyPassword);
        }

        public void UpdateMyInformation(UpdateInformationModel model)
        {
            IValidator validator = new FluentValidator<UpdateInformationModel, UpdateInformationModelValidationRules>(model);

            var validationResults = validator.Validate();

            if (!validator.IsValid)
            {
                throw new ValidationException(Messages.DangerInvalidEntitiy)
                {
                    ValidationResult = validationResults
                };
            }

            var identity = (AnkaCMSIdentity)Thread.CurrentPrincipal.Identity;
            var user = _repositoryUser
                .Join(x => x.Person)
                .FirstOrDefault(e => e.Id == identity.UserId);

            if (user == null)
            {
                throw new NotFoundException(Messages.DangerRecordNotFound);
            }


            var person = user.Person;

            if (model.Username != user.Username)
            {
                if (_repositoryUser.Get().Any(p => p.Username == model.Username))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Username));
                }
            }
            if (model.Email != user.Email)
            {
                if (_repositoryUser.Get().Any(p => p.Email == model.Email))
                {
                    throw new DuplicateException(string.Format(Messages.DangerFieldDuplicated, Dictionary.Email));
                }
            }


            var versionHistoryPerson = _repositoryPersonHistory.Get().Where(e => e.ReferenceId == person.Id).Max(t => t.Version);

            var personHistory = person.CreateMapped<Person, PersonHistory>();
            personHistory.Id = GuidHelper.NewGuid();
            personHistory.ReferenceId = user.Id;
            personHistory.CreatorId = user.Creator.Id;
            personHistory.IsDeleted = false;
            personHistory.Version = versionHistoryPerson + 1;
            personHistory.RestoreVersion = 0;
            _repositoryPersonHistory.Add(personHistory, true);

            person.FirstName = model.FirstName;
            person.LastName = model.LastName;
            person.Biography = model.Biography;


            person.LastModificationTime = DateTime.Now;
            person.LastModifierId = user.Id;
            var versionPerson = user.Version;
            person.Version = versionPerson + 1;
            _repositoryPerson.Update(person, true);

            var versionHistoryUser = _repositoryUserHistory.Get().Where(e => e.ReferenceId == user.Id).Max(t => t.Version);

            var userHistory = user.CreateMapped<User, UserHistory>();
            userHistory.Id = GuidHelper.NewGuid();
            userHistory.ReferenceId = user.Id;
            userHistory.CreatorId = user.Creator.Id;
            userHistory.IsDeleted = false;
            userHistory.Version = versionHistoryUser + 1;
            userHistory.RestoreVersion = 0;
            _repositoryUserHistory.Add(userHistory, true);

            user.Username = model.Username;
            user.Email = model.Email;


            user.LastModificationTime = DateTime.Now;
            user.LastModifier = user;
            var version = user.Version;
            user.Version = version + 1;
            var affectedUser = _repositoryUser.Update(user, true);
            if (!_serviceMain.ApplicationSettings.SendMailAfterUpdateUserInformation) return;

            var emailUser = new EmailUser
            {
                Username = affectedUser.Username,
                Password = string.Empty,
                CreationTime = affectedUser.CreationTime,
                Email = affectedUser.Email,
                FirstName = affectedUser.Person.FirstName,
                LastName = affectedUser.Person.LastName
            };
            var emailSender = new EmailSender(_smtp,_serviceMain);
            emailSender.SendEmailToUser(emailUser, EmailTypeOption.UpdateMyInformation);
        }




    }
}