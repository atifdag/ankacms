using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;

namespace AnkaCMS.SetupConsoleApp.Installation
{
    public static class UserInstallation
    {

        public static List<Tuple<string, string, string,string>> UserList = new List<Tuple<string, string, string, string>>
        {
            Tuple.Create("Yönetici","Kullanıcı","yonetici.kullanici", "MANAGER"),
            Tuple.Create("Editör","Kullanıcı","editor.kullanici","EDITOR"),
            Tuple.Create("Yazar","Kullanıcı","yazar.kullanici","AUTHOR"),
            Tuple.Create("Abone","Kullanıcı","abone.kullanici","SUBSCRIBER"),
            Tuple.Create("Varsayılan","Kullanıcı","varsayilan.kullanici","DEFAULTROLE"),
        };

        public static void Install(IServiceProvider provider)
        {
            var repositoryPerson = provider.GetService<IRepository<Person>>();
            var repositoryPersonHistory = provider.GetService<IRepository<PersonHistory>>();

            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryUserHistory = provider.GetService<IRepository<UserHistory>>();

            var setupProjectRootPath = AppContext.BaseDirectory;
            if (AppContext.BaseDirectory.Contains("bin"))
            {
                setupProjectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
            }


            var setupProjectUserFiles = Path.Combine(setupProjectRootPath, "UserFiles");


            foreach (var directory in Directory.GetDirectories(setupProjectUserFiles))
            {
                if (directory.Contains("Public") || directory.Contains("DeletedUsers")) continue;
                var directoryInfo = new DirectoryInfo(directory);
                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(directory);
            }


            var apiProjectUserFiles = setupProjectUserFiles.Replace("AnkaCMS.SetupConsoleApp", setupProjectUserFiles.Contains("\\") ? "AnkaCMS.WebApi\\wwwroot" : "AnkaCMS.WebApi/wwwroot");

            foreach (var directory in Directory.GetDirectories(apiProjectUserFiles))
            {
                if (directory.Contains("Public") || directory.Contains("DeletedUsers")) continue;
                var directoryInfo = new DirectoryInfo(directory);
                foreach (var file in directoryInfo.GetFiles())
                {
                    file.Delete();
                }
                foreach (var dir in directoryInfo.GetDirectories())
                {
                    dir.Delete(true);
                }
                Directory.Delete(directory);
            }


            var setupProjectPublicUserFiles = Path.Combine(setupProjectUserFiles, "Public");

            var firstPerson = new Person
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                IdentityCode = "12345678901",
                FirstName = "Atıf",
                LastName = "DAĞ",
                BirthDate = DateTime.MinValue
            };

            var firstPassword = firstPerson.FirstName.ToStringForSeo().Substring(0, 1) + firstPerson.LastName.ToStringForSeo().Substring(0, 1) + ".Anka2019";

            var firstUser = new User
            {
                Id = GuidHelper.NewGuid(),
                CreationTime = DateTime.Now,
                LastModificationTime = DateTime.Now,
                DisplayOrder = 1,
                Version = 1,
                IsApproved = true,
                Username = "atif.dag",
                Password = firstPassword.ToSha512(),
                Email = "web@ankacms.com.tr",
                Person = firstPerson
            };

            firstPerson.CreatorId = firstUser.Id;
            firstPerson.LastModifierId = firstUser.Id;

            var affectedFirstPerson = repositoryPerson.Add(firstPerson, true);

            firstUser.Creator = firstUser;
            firstUser.LastModifier = firstUser;

            var affectedFirstUser = repositoryUser.Add(firstUser, true);

            var firstPersonHistory = affectedFirstPerson.CreateMapped<Person, PersonHistory>();
            firstPersonHistory.Id = GuidHelper.NewGuid();
            firstPersonHistory.ReferenceId = affectedFirstPerson.Id;
            firstPersonHistory.IsDeleted = false;
            firstPersonHistory.RestoreVersion = 0;

            var firstUserHistory = affectedFirstUser.CreateMapped<User, UserHistory>();
            firstUserHistory.Id = GuidHelper.NewGuid();
            firstUserHistory.PersonId = affectedFirstUser.Person.Id;
            firstUserHistory.ReferenceId = affectedFirstUser.Id;
            firstUserHistory.CreatorId = affectedFirstUser.Creator.Id;
            firstUserHistory.IsDeleted = false;
            firstUserHistory.RestoreVersion = 0;

            repositoryPersonHistory.Add(firstPersonHistory, true);
            repositoryUserHistory.Add(firstUserHistory, true);

            var firstUserDirectoryPath = Path.Combine(setupProjectUserFiles, affectedFirstUser.Id.ToString());
            if (!Directory.Exists(firstUserDirectoryPath))
            {
                Directory.CreateDirectory(firstUserDirectoryPath);
                FileHelper.CopyDirectory(setupProjectPublicUserFiles, firstUserDirectoryPath);
            }

            

            var counterUser = 2;

            var userListCount = UserList.Count + 1;

            foreach (var (item1, item2, item3, item4) in UserList)
            {
                var person = new Person
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterUser,
                    Version = 1,
                    IsApproved = true,
                    IdentityCode = "1234567890"+ counterUser,
                    FirstName = item1,
                    LastName = item2,
                    BirthDate = DateTime.MinValue,
                    CreatorId = firstUser.Id,
                    LastModifierId = firstUser.Id
                };
                var password = person.FirstName.ToStringForSeo().Substring(0, 1) + person.LastName.ToStringForSeo().Substring(0, 1) + ".Kaya2019";
                var affectedPerson = repositoryPerson.Add(person, true);

                var user = new User
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = 1,
                    Version = 1,
                    IsApproved = true,
                    Username = item3,
                    Password = password.ToSha512(),
                    Email = item3+ "@ankacms.com.tr",
                    Person = affectedPerson,
                    Creator = firstUser,
                    LastModifier = firstUser
                };


                var affectedUser = repositoryUser.Add(user, true);

                var personHistory = affectedPerson.CreateMapped<Person, PersonHistory>();
                personHistory.Id = GuidHelper.NewGuid();
                personHistory.ReferenceId = affectedPerson.Id;
                personHistory.IsDeleted = false;
                personHistory.RestoreVersion = 0;

                var userHistory = affectedUser.CreateMapped<User, UserHistory>();
                userHistory.Id = GuidHelper.NewGuid();
                userHistory.PersonId = affectedUser.Person.Id;
                userHistory.ReferenceId = affectedUser.Id;
                userHistory.CreatorId = affectedUser.Creator.Id;
                userHistory.IsDeleted = false;
                userHistory.RestoreVersion = 0;

                repositoryPersonHistory.Add(personHistory, true);
                repositoryUserHistory.Add(userHistory, true);

                var userDirectoryPath = Path.Combine(setupProjectUserFiles, affectedUser.Id.ToString());
                if (!Directory.Exists(userDirectoryPath))
                {
                    Directory.CreateDirectory(userDirectoryPath);
                    FileHelper.CopyDirectory(setupProjectPublicUserFiles, userDirectoryPath);
                }

                Console.WriteLine(counterUser + @"/" + userListCount + @" User (" + affectedUser.Username + @")");
                counterUser++;
            }

            
            FileHelper.CopyDirectory(setupProjectUserFiles, apiProjectUserFiles);

        }
    }
}

