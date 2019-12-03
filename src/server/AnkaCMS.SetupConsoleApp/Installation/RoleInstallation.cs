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
    public static class RoleInstallation
    {
        private static readonly List<Role> Items = new List<Role>
        {
            new Role {Code = "DEVELOPER", Name = "Geliştirici", Level = 1},
            new Role {Code = "MANAGER", Name = "Yönetici", Level = 100},
            new Role {Code = "EDITOR", Name = "Editör", Level = 200},
            new Role {Code = "AUTHOR", Name = "Yazar", Level = 300},
            new Role {Code = "SUBSCRIBER", Name = "Abone", Level = 400},
            new Role {Code = "DEFAULTROLE", Name = "Varsayılan Kullanıcı", Level = 1000}
        };


        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var firstUser = repositoryUser.Get(x => x.Username == "atif.dag");

            var listRole = new List<Role>();
            var listRoleHistory = new List<RoleHistory>();
            var listRoleUserLine = new List<RoleUserLine>();
            var listRoleUserLineHistory = new List<RoleUserLineHistory>();

            

            var itemsCount = Items.Count;
            var itemsCounter = 1;


            foreach (var item in Items)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = 1;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = firstUser;
                item.LastModifier = firstUser;
                listRole.Add(item);

                var itemHistory = item.CreateMapped<Role, RoleHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;

                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listRoleHistory.Add(itemHistory);

                Console.WriteLine(itemsCounter + @"/" + itemsCount + @" Role (" + item.Code + @")");

                itemsCounter++;

            }
            
            var counterUserRoleList = 1;
            var userRoleListCount = UserInstallation.UserList.Count;

            var firstLine = new RoleUserLine
            {
                Id = GuidHelper.NewGuid(),
                User = firstUser,
                Role = listRole.FirstOrDefault(x => x.Code == "DEVELOPER"),
                Creator = firstUser,
                CreationTime = DateTime.Now,
                LastModifier = firstUser,
                LastModificationTime = DateTime.Now,
                DisplayOrder = counterUserRoleList,
                Version = 1

            };

            listRoleUserLine.Add(firstLine);
            var firstLineHistory = firstLine.CreateMapped<RoleUserLine, RoleUserLineHistory>();
            firstLineHistory.Id = GuidHelper.NewGuid();
            firstLineHistory.RoleId = firstLine.Role.Id;
            firstLineHistory.UserId = firstLine.User.Id;
            firstLineHistory.ReferenceId = firstLine.Id;
            firstLineHistory.CreatorId = firstLine.Creator.Id;
            firstLineHistory.RestoreVersion = 0;
            listRoleUserLineHistory.Add(firstLineHistory);


            foreach (var (item1, item2, item3, item4) in UserInstallation.UserList)
            {
                var user = repositoryUser.Get(x => x.Username == item3);
                var role = listRole.FirstOrDefault(x => x.Code == item4);

                var line = new RoleUserLine
                {
                    Id = GuidHelper.NewGuid(),
                    User = user,
                    Role = role,
                    Creator = firstUser,
                    CreationTime = DateTime.Now,
                    LastModifier = firstUser,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterUserRoleList,
                    Version = 1

                };

                listRoleUserLine.Add(line);
                var lineHistory = line.CreateMapped<RoleUserLine, RoleUserLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.RoleId = line.Role.Id;
                lineHistory.UserId = line.User.Id;
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreatorId = line.Creator.Id;
                lineHistory.RestoreVersion = 0;
                listRoleUserLineHistory.Add(lineHistory);

                Console.WriteLine(counterUserRoleList + @"/" + userRoleListCount + @" RoleUserLine (" + user.Username + @" - " + role.Code + @")");

                counterUserRoleList++;

            }


            unitOfWork.Context.AddRange(listRole);
            unitOfWork.Context.AddRange(listRoleHistory);
            unitOfWork.Context.AddRange(listRoleUserLine);
            unitOfWork.Context.AddRange(listRoleUserLineHistory);
            unitOfWork.Context.SaveChanges();

        }

    }
}
