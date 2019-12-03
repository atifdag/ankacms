using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AnkaCMS.Data.DataAccess.EntityFramework;

namespace AnkaCMS.SetupConsoleApp.Installation
{
    public static class PermissionInstallation
    {
        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryRole = provider.GetService<IRepository<Role>>();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");
            var developerRole = repositoryRole.Get(x => x.Code == "DEVELOPER");
            var defaultRole = repositoryRole.Get(x => x.Code == "DEFAULTROLE");


            var listPermission = new List<Permission>();
            var listPermissionHistory = new List<PermissionHistory>();
            var listRolePermissionLine = new List<RolePermissionLine>();
            var listRolePermissionLineHistory = new List<RolePermissionLineHistory>();


            var defaultUserPermissions = new List<Permission>
            {
                new Permission {ControllerName = "Authentication", ActionName = "MyProfile"},
                new Permission {ControllerName = "Authentication", ActionName = "UpdateMyInformation"},
                new Permission {ControllerName = "Authentication", ActionName = "UpdateMyPassword"}
            };

            var totalCountDefaultUser = defaultUserPermissions.Count;
            var counterDefaultUser = 1;


            foreach (var item in defaultUserPermissions)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = 1;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = user;
                item.LastModifier = user;
                item.Code = item.ControllerName + item.ActionName;
                item.Name = item.ControllerName + " " + item.ActionName;
                listPermission.Add(item);

                var itemHistory = item.CreateMapped<Permission, PermissionHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listPermissionHistory.Add(itemHistory);

                var line = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = item,
                    Role = defaultRole,
                    Creator = user,
                    CreationTime = DateTime.Now,
                    LastModifier = user,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = 1,
                    Version = 1,

                };

                listRolePermissionLine.Add(line);
                var lineHistory = line.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreatorId = line.Creator.Id;
                lineHistory.RestoreVersion = 0;
                listRolePermissionLineHistory.Add(lineHistory);
                Console.WriteLine(counterDefaultUser + @"/" + totalCountDefaultUser + @" RolePermissionLine (" + defaultRole.Code + @" - " + item.Code + @")");
                counterDefaultUser++;

            }

            var developerPermissions = new List<Permission>();

            var actions = new List<string>
            {
                "List",
                "Filter",
                "Detail",
                "Add",
                "Update",
                "Delete",
                "KeysAndValues",
            };

            var developerControllers = new List<string>
            {
                "Category",
                "Content",
                "Language",
                "Menu",
                "Parameter",
                "ParameterGroup",
                "Part",
                "Permission",
                "Person",
                "Role",
                "User"
            };
            var displayOrder = 1;
            foreach (var controller in developerControllers)
            {
                foreach (var action in actions)
                {
                    developerPermissions.Add(new Permission
                    {
                        Id = GuidHelper.NewGuid(),
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        DisplayOrder = displayOrder,
                        Version = 1,
                        IsApproved = true,
                        Creator = user,
                        LastModifier = user,
                        Code = controller + action,
                        Name = controller + " " + action,
                        ControllerName = controller,
                        ActionName = action,
                        Description = string.Empty
                    });
                    displayOrder++;
                }
            }


            var totalCountDeveloper = developerPermissions.Count;
            var counterDeveloper = 1;

            foreach (var item in developerPermissions)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterDeveloper;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = user;
                item.LastModifier = user;
                item.Code = item.ControllerName + item.ActionName;
                item.Name = item.ControllerName + " " + item.ActionName;

                var itemHistory = item.CreateMapped<Permission, PermissionHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;
                listPermission.Add(item);
                listPermissionHistory.Add(itemHistory);

                var line = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = item,
                    Role = developerRole,
                    Creator = user,
                    CreationTime = DateTime.Now,
                    LastModifier = user,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = 1,
                    Version = 1,

                };

                listRolePermissionLine.Add(line);
                var lineHistory = line.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.RoleId = line.Role.Id;
                lineHistory.PermissionId = line.Permission.Id;
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreatorId = line.Creator.Id;
                lineHistory.RestoreVersion = 0;
                listRolePermissionLineHistory.Add(lineHistory);

                Console.WriteLine(counterDeveloper + @"/" + totalCountDeveloper + @" RolePermissionLine (" + developerRole.Code + @" - " + item.Code + @")");

                counterDeveloper++;
            }


            var otherPermissions = new List<RolePermissionLine>
            {
                new RolePermissionLine {Role = new Role {Code = "DEVELOPER"}, Permission = new Permission {ControllerName = "Cache", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "DEVELOPER"}, Permission = new Permission {ControllerName = "Cache", ActionName = "Delete"}},

                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Role", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "User", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Language", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Category", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Content", ActionName = "KeysAndValues"}},

                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Part", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Part", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Part", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Part", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "MANAGER"}, Permission = new Permission {ControllerName = "Part", ActionName = "KeysAndValues"}},

                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Language", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "Add"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "Delete"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Part", ActionName = "List"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Part", ActionName = "Filter"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Part", ActionName = "Detail"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Part", ActionName = "Update"}},
                new RolePermissionLine {Role = new Role {Code = "EDITOR"}, Permission = new Permission {ControllerName = "Part", ActionName = "KeysAndValues"}},

                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Language", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Category", ActionName = "KeysAndValues"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentList"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentFilter"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentDetail"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentAdd"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentUpdate"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentDelete"}},
                new RolePermissionLine {Role = new Role {Code = "AUTHOR"}, Permission = new Permission {ControllerName = "Content", ActionName = "MyContentKeysAndValues"}},

            };


            var totalOtherPermissions = otherPermissions.Count;
            var counterOtherPermissions = 1;

            foreach (var line in otherPermissions)
            {
                var itemRole = repositoryRole.Get(x => x.Code == line.Role.Code);

                var itemPermission = listPermission.FirstOrDefault(x => x.Code == line.Permission.ControllerName + line.Permission.ActionName);

                if (itemPermission == null)
                {
                    var newPermission = new Permission
                    {
                        Id = GuidHelper.NewGuid(),
                        Code = line.Permission.ControllerName + line.Permission.ActionName,
                        Name = line.Permission.ControllerName + " " + line.Permission.ActionName,
                        ControllerName = line.Permission.ControllerName,
                        ActionName = line.Permission.ActionName,
                        Creator = user,
                        IsApproved = true,
                        LastModifier = user,
                        DisplayOrder = counterOtherPermissions,
                        Description = "",
                        CreationTime = DateTime.Now,
                        LastModificationTime = DateTime.Now,
                        Version = 1
                    };

                    listPermission.Add(newPermission);
                    itemPermission = newPermission;
                }

                var itemPermissionHistory = itemPermission.CreateMapped<Permission, PermissionHistory>();
                itemPermissionHistory.Id = GuidHelper.NewGuid();
                itemPermissionHistory.ReferenceId = itemPermission.Id;
                itemPermissionHistory.CreationTime = DateTime.Now;
                itemPermissionHistory.CreatorId = user.Id;
                itemPermissionHistory.IsDeleted = false;
                itemPermissionHistory.RestoreVersion = 0;
                listPermissionHistory.Add(itemPermissionHistory);

                var affectedRolePermissionLine = new RolePermissionLine
                {
                    Id = GuidHelper.NewGuid(),
                    Role = itemRole,
                    Permission = itemPermission,
                    Creator = user,
                    LastModifier = user,
                    DisplayOrder = counterOtherPermissions,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    Version = 1
                };



                listRolePermissionLine.Add(affectedRolePermissionLine);

                var affectedRolePermissionLineHistory = affectedRolePermissionLine.CreateMapped<RolePermissionLine, RolePermissionLineHistory>();
                affectedRolePermissionLineHistory.Id = GuidHelper.NewGuid();
                affectedRolePermissionLineHistory.PermissionId = affectedRolePermissionLine.Permission.Id;
                affectedRolePermissionLineHistory.RoleId = affectedRolePermissionLine.Role.Id;
                affectedRolePermissionLineHistory.CreationTime = DateTime.Now;
                affectedRolePermissionLineHistory.CreatorId = user.Id;
                affectedRolePermissionLineHistory.ReferenceId = affectedRolePermissionLine.Id;
                affectedRolePermissionLineHistory.RestoreVersion = 0;

                listRolePermissionLineHistory.Add(affectedRolePermissionLineHistory);
                Console.WriteLine(counterOtherPermissions + @"/" + totalOtherPermissions + @" RolePermissionLine (" + itemRole.Code + @" - " + itemPermission.Code + @")");

                counterOtherPermissions++;
            }

            unitOfWork.Context.AddRange(listPermission);
            unitOfWork.Context.AddRange(listPermissionHistory);
            unitOfWork.Context.AddRange(listRolePermissionLine);
            unitOfWork.Context.AddRange(listRolePermissionLineHistory);
            unitOfWork.Context.SaveChanges();

        }
    }
}