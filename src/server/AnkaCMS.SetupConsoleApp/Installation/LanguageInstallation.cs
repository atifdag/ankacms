using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using AnkaCMS.Data.DataAccess.EntityFramework;

namespace AnkaCMS.SetupConsoleApp.Installation
{
    public static class LanguageInstallation
    {
        private static readonly List<Language> Items = new List<Language>
        {
            new Language {Code = "tr", Name = "Türkçe"},
            new Language {Code = "en", Name = "English"},
            new Language {Code = "ar", Name = "العربية"},
            new Language {Code = "ru", Name = "русский"},
            new Language {Code = "fa", Name = "فارسی"},
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var listLanguage = new List<Language>();
            var listLanguageHistory = new List<LanguageHistory>();

           
            var counterLanguage = 1;
            var itemsCount = Items.Count;

            foreach (var item in Items)
            {
                item.Id = GuidHelper.NewGuid();
                item.CreationTime = DateTime.Now;
                item.LastModificationTime = DateTime.Now;
                item.DisplayOrder = counterLanguage;
                item.Version = 1;
                item.IsApproved = true;
                item.Creator = user;
                item.LastModifier = user;
                listLanguage.Add(item);

                var itemHistory = item.CreateMapped<Language, LanguageHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;

                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listLanguageHistory.Add(itemHistory);
                Console.WriteLine(counterLanguage + @"/" + itemsCount + @" Language (" + item.Code + @")");
                counterLanguage++;
            }

            unitOfWork.Context.AddRange(listLanguage);
            unitOfWork.Context.AddRange(listLanguageHistory);
            unitOfWork.Context.SaveChanges();
        }
    }
}
