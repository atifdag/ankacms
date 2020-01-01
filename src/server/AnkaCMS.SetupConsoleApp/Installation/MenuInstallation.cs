using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Globalization;
using AnkaCMS.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using AnkaCMS.Data.DataAccess.EntityFramework;

namespace AnkaCMS.SetupConsoleApp.Installation
{
    public static class MenuInstallation
    {
        private static List<Tuple<string, string>> PermissionMenus => new List<Tuple<string, string>>
        {

            Tuple.Create("CategoryList","CategoryManagement"),
            Tuple.Create("CategoryList","CategoryList"),
            Tuple.Create("CategoryAdd","CategoryAdd"),

            Tuple.Create("ContentList","ContentManagement"),
            Tuple.Create("ContentList","ContentList"),
            Tuple.Create("ContentAdd","ContentAdd"),

            Tuple.Create("MenuList","MenuManagement"),
            Tuple.Create("MenuList","MenuList"),
            Tuple.Create("MenuAdd","MenuAdd"),

            Tuple.Create("LanguageList","LanguageManagement"),
            Tuple.Create("LanguageList","LanguageList"),
            Tuple.Create("LanguageAdd","LanguageAdd"),

            Tuple.Create("ParameterGroupList","ParameterGroupManagement"),
            Tuple.Create("ParameterGroupList","ParameterGroupList"),
            Tuple.Create("ParameterGroupAdd","ParameterGroupAdd"),

            Tuple.Create("ParameterList","ParameterManagement"),
            Tuple.Create("ParameterList","ParameterList"),
            Tuple.Create("ParameterAdd","ParameterAdd"),

            Tuple.Create("PartList","PartManagement"),
            Tuple.Create("PartList","PartList"),
            Tuple.Create("PartAdd","PartAdd"),

            Tuple.Create("PermissionList","PermissionManagement"),
            Tuple.Create("PermissionList","PermissionList"),
            Tuple.Create("PermissionAdd","PermissionAdd"),

            Tuple.Create("RoleList","RoleManagement"),
            Tuple.Create("RoleList","RoleList"),
            Tuple.Create("RoleAdd","RoleAdd"),

            Tuple.Create("UserList","UserManagement"),
            Tuple.Create("UserList","UserList"),
            Tuple.Create("UserAdd","UserAdd"),
        };


        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryPermission = provider.GetService<IRepository<Permission>>();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var listMenu = new List<Menu>();
            var listMenuHistory = new List<MenuHistory>();
            var listPermissionMenuLine = new List<PermissionMenuLine>();
            var listPermissionMenuLineHistory = new List<PermissionMenuLineHistory>();

            var rootMenu = new Menu
            {
                Id = GuidHelper.NewGuid(),
                Code = "ADMINROOTMENU",
                Address = "#",

                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                Creator = user,
                LastModifier = user,
                Name = "-"
            };

            rootMenu.ParentMenu = rootMenu;

            var rootMenuHistory = rootMenu.CreateMapped<Menu, MenuHistory>();
            rootMenuHistory.Id = GuidHelper.NewGuid();
            rootMenuHistory.ReferenceId = rootMenu.Id;
            rootMenuHistory.CreatorId = rootMenu.Creator.Id;

            rootMenuHistory.ParentMenuId = rootMenu.ParentMenu.Id;
            rootMenuHistory.IsDeleted = false;
            rootMenuHistory.RestoreVersion = 0;
            listMenu.Add(rootMenu);
            listMenuHistory.Add(rootMenuHistory);


            var developerRootMenus = new List<Menu>
            {
                new Menu {Address = "#", Code = "CategoryManagement", Name = Dictionary.CategoryManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "ContentManagement", Name =Dictionary.ContentManagement, ParentMenu = rootMenu, Icon = "pi pi-align-justify"},
                new Menu {Address = "#", Code = "LanguageManagement", Name =Dictionary.LanguageManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "MenuManagement", Name =Dictionary.MenuManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "ParameterGroupManagement", Name =Dictionary.ParameterGroupManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "ParameterManagement", Name =Dictionary.ParameterManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "PartManagement", Name =Dictionary.PartManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "PermissionManagement", Name =Dictionary.PermissionManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "RoleManagement", Name =Dictionary.RoleManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
                new Menu {Address = "#", Code = "UserManagement", Name =Dictionary.UserManagement, ParentMenu = rootMenu, Icon = "pi pi-th-folder"},
            };

            var totalDeveloperRootMenus = developerRootMenus.Count;
            var counterDeveloperRootMenus = 1;

            foreach (var item in developerRootMenus)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = 1;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = user;
                item.LastModifier = user;
                listMenu.Add(item);

                var itemHistory = item.CreateMapped<Menu, MenuHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.ParentMenuId = item.Id;

                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;
                listMenuHistory.Add(itemHistory);

                Console.WriteLine(counterDeveloperRootMenus + @"/" + totalDeveloperRootMenus + @" Menu (" + item.Code + @")");
                counterDeveloperRootMenus++;
            }


            var developerChildMenus = new List<Menu>
            {
                new Menu {Address = "/Category/Add", Code = "CategoryAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="CategoryManagement")},
                new Menu {Address = "/Category/List", Code = "CategoryList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="CategoryManagement")},

                new Menu {Address = "/Content/Add", Code = "ContentAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ContentManagement")},
                new Menu {Address = "/Content/List", Code = "ContentList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ContentManagement")},

                new Menu {Address = "/Language/Add", Code = "LanguageAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="LanguageManagement")},
                new Menu {Address = "/Language/List", Code = "LanguageList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="LanguageManagement")},

                new Menu {Address = "/Menu/Add", Code = "MenuAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="MenuManagement")},
                new Menu {Address = "/Menu/List", Code = "MenuList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="MenuManagement")},
                
                new Menu {Address = "/ParameterGroup/Add", Code = "ParameterGroupAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ParameterGroupManagement")},
                new Menu {Address = "/ParameterGroup/List", Code = "ParameterGroupList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ParameterGroupManagement")},

                new Menu {Address = "/Parameter/Add", Code = "ParameterAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ParameterManagement")},
                new Menu {Address = "/Parameter/List", Code = "ParameterList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="ParameterManagement")},

                new Menu {Address = "/Part/Add", Code = "PartAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="PartManagement")},
                new Menu {Address = "/Part/List", Code = "PartList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="PartManagement")},

                new Menu {Address = "/Permission/Add", Code = "PermissionAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="PermissionManagement")},
                new Menu {Address = "/Permission/List", Code = "PermissionList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="PermissionManagement")},

                new Menu {Address = "/Role/Add", Code = "RoleAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="RoleManagement")},
                new Menu {Address = "/Role/List", Code = "RoleList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="RoleManagement")},

                new Menu {Address = "/User/Add", Code = "UserAdd", Name = Dictionary.Add, Icon ="pi pi-plus", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="UserManagement")},
                new Menu {Address = "/User/List", Code = "UserList", Name = Dictionary.List, Icon ="pi pi-list", ParentMenu = listMenu.FirstOrDefault(x=>x.Code=="UserManagement")},

            };

            var developerChildMenusCount = developerChildMenus.Count;
            var developerChildMenusCounter = 1;


            foreach (var item in developerChildMenus)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = 1;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = user;
                item.LastModifier = user;
                listMenu.Add(item);

                var itemHistory = item.CreateMapped<Menu, MenuHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.ParentMenuId = item.Id;

                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;
                listMenuHistory.Add(itemHistory);
                Console.WriteLine(developerChildMenusCounter + @"/" + developerChildMenusCount + @" Menu (" + item.Code + @")");
                developerChildMenusCounter++;

            }

            PermissionMenuInstall(user, listMenu, listPermissionMenuLine, listPermissionMenuLineHistory, repositoryPermission);

            var otherPermissionMenuLines = new List<PermissionMenuLine>
            {
                new PermissionMenuLine { Menu = new Menu{ Name = Dictionary.MyContentManagement}, Permission =  new Permission {ControllerName = "Content", ActionName = "MyContentList"}},
                new PermissionMenuLine { Menu = new Menu{ Name = Dictionary.CacheManagement}, Permission =  new Permission {ControllerName = "Cache", ActionName = "List"}},
            };


            var totalOtherPermissionMenuLines = otherPermissionMenuLines.Count;
            var counterOtherPermissionMenuLines = 1;

            foreach (var otherPermissionMenuLine in otherPermissionMenuLines)
            {
                var itemPermission = repositoryPermission.Get(x => x.ControllerName == otherPermissionMenuLine.Permission.ControllerName && x.ActionName == otherPermissionMenuLine.Permission.ActionName);

                var affectedMenu = new Menu
                {
                    Id = GuidHelper.NewGuid(),
                    Code = otherPermissionMenuLine.Permission.ControllerName +
                           otherPermissionMenuLine.Permission.ActionName,
                    Name = otherPermissionMenuLine.Menu.Name,
                    Address = "/" + otherPermissionMenuLine.Permission.ControllerName + "/" +
                              otherPermissionMenuLine.Permission.ActionName,
                    Creator = user,
                    IsApproved = true,
                    LastModifier = user,
                    DisplayOrder = counterOtherPermissionMenuLines,
                    CreationTime = DateTime.Now,
                    Version = 1,
                    Description = string.Empty,
                    LastModificationTime = DateTime.Now,
                    Icon = string.Empty,
                    ParentMenu = rootMenu
                };

                listMenu.Add(affectedMenu);




                var menuHistory = affectedMenu.CreateMapped<Menu, MenuHistory>();
                menuHistory.Id = GuidHelper.NewGuid();
                menuHistory.ReferenceId = affectedMenu.Id;
                menuHistory.CreationTime = DateTime.Now;
                menuHistory.CreatorId = user.Id;
                menuHistory.IsDeleted = false;
                menuHistory.RestoreVersion = 0;
                menuHistory.ParentMenuId = affectedMenu.ParentMenu.Id;
                listMenuHistory.Add(menuHistory);

                var addedLine = new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Menu = affectedMenu,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModificationTime = DateTime.Now,
                    LastModifier = user,
                    DisplayOrder = counterOtherPermissionMenuLines,
                    Version = 1
                };

                listPermissionMenuLine.Add(addedLine);

                var lineHistory = addedLine.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = addedLine.Id;
                lineHistory.CreatorId = addedLine.Creator.Id;
                lineHistory.PermissionId = addedLine.Permission.Id;
                lineHistory.MenuId = addedLine.Menu.Id;
                lineHistory.RestoreVersion = 0;
                listPermissionMenuLineHistory.Add(lineHistory);

                Console.WriteLine(counterOtherPermissionMenuLines + @"/" + totalOtherPermissionMenuLines + @" Menu (" + affectedMenu.Code + @")");
                counterOtherPermissionMenuLines++;
            }

            unitOfWork.Context.AddRange(listMenu);
            unitOfWork.Context.AddRange(listMenuHistory);
            unitOfWork.Context.AddRange(listPermissionMenuLine);
            unitOfWork.Context.AddRange(listPermissionMenuLineHistory);
            unitOfWork.Context.SaveChanges();

        }

        private static void PermissionMenuInstall(User user, IReadOnlyCollection<Menu> repositoryMenu, ICollection<PermissionMenuLine> repositoryPermissionMenuLine, ICollection<PermissionMenuLineHistory> repositoryPermissionMenuLineHistory, IRepository<Permission> repositoryPermission)
        {

            var permissionMenusCount = PermissionMenus.Count;
            var permissionMenusCounter = 1;

            foreach (var (item1, item2) in PermissionMenus)
            {
                var itemPermission = repositoryPermission.Get(x => x.Code == item1);
                var itemMenu = repositoryMenu.FirstOrDefault(x => x.Code == item2);

                var addedLine = new PermissionMenuLine
                {
                    Id = GuidHelper.NewGuid(),
                    Permission = itemPermission,
                    Menu = itemMenu,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModificationTime = DateTime.Now,
                    LastModifier = user,
                    DisplayOrder = 1,
                    Version = 1
                };

                repositoryPermissionMenuLine.Add(addedLine);

                var lineHistory = addedLine.CreateMapped<PermissionMenuLine, PermissionMenuLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = addedLine.Id;
                lineHistory.CreatorId = addedLine.Creator.Id;
                lineHistory.PermissionId = addedLine.Permission.Id;
                lineHistory.MenuId = addedLine.Menu.Id;
                lineHistory.RestoreVersion = 0;
                repositoryPermissionMenuLineHistory.Add(lineHistory);
                Console.WriteLine(permissionMenusCounter + @"/" + permissionMenusCount + @" PermissionMenuLine (" + itemPermission.Code + @" - " + itemMenu.Code + @")");
                permissionMenusCounter++;
            }
        }

    }
}