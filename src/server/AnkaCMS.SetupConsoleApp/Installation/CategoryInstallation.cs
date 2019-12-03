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
    public static class CategoryInstallation
    {
        private static readonly List<Tuple<string, string, int>> Items = new List<Tuple<string, string, int>>
        {
            Tuple.Create("KURUMSAL", "Kurumsal",1),
            Tuple.Create("HABERLER", "Haberler",2),
            Tuple.Create("BAGLANTILAR", "Bağlantılar",3)
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryLanguage = provider.GetService<IRepository<Language>>();
            var languages = repositoryLanguage.Get().Where(x => x.IsApproved).ToList();

            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var repositoryCategory = new List<Category>();
            var repositoryCategoryHistory = new List<CategoryHistory>();
            var repositoryCategoryLanguageLine = new List<CategoryLanguageLine>();
            var repositoryCategoryLanguageLineHistory = new List<CategoryLanguageLineHistory>();

            

            var totalCount = Items.Count * languages.Count;
            var counterCategory = 1;

            foreach (var (item1, item2, item3) in Items)
            {
                var item = new Category
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModifier = user,
                    LastModificationTime = DateTime.Now
                };

                repositoryCategory.Add(item);
                var itemHistory = item.CreateMapped<Category, CategoryHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                repositoryCategoryHistory.Add(itemHistory);

                foreach (var language in languages)
                {
                    var line = new CategoryLanguageLine
                    {
                        Id = GuidHelper.NewGuid(),
                        Code = item1 + " " + language.Code,
                        Name = item2 + " " + language.Code,
                        DisplayOrder = item3,
                        Language = language,
                        Category = item,
                        CreationTime = DateTime.Now,
                        Creator = user,
                        LastModificationTime = DateTime.Now,
                        LastModifier = user,
                        Version = 1,
                        IsApproved = true
                    };

                    repositoryCategoryLanguageLine.Add(line);

                    var lineHistory = line.CreateMapped<CategoryLanguageLine, CategoryLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = line.Id;
                    lineHistory.CreatorId = line.Creator.Id;
                    lineHistory.CategoryId = line.Category.Id;
                    lineHistory.LanguageId = line.Language.Id;
                    lineHistory.RestoreVersion = 0;
                    repositoryCategoryLanguageLineHistory.Add(lineHistory);

                    Console.WriteLine(counterCategory + @"/" + totalCount + @" CategoryLanguageLine (" + line.Code + @")");
                    counterCategory++;

                }

            }

            unitOfWork.Context.AddRange(repositoryCategory);
            unitOfWork.Context.AddRange(repositoryCategoryHistory);
            unitOfWork.Context.AddRange(repositoryCategoryLanguageLine);
            unitOfWork.Context.AddRange(repositoryCategoryLanguageLineHistory);
            unitOfWork.Context.SaveChanges();

        }
    }
}
