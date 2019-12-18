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
    public static class PartInstallation
    {
        public static List<Tuple<string, string, int>> PartTuples = new List<Tuple<string, string, int>>
        {
            Tuple.Create("MANSET", "Manşet",1),
            Tuple.Create("MANSETALTI", "Manşet Altı",2),
            Tuple.Create("ONECIKANLAR", "Öne Çıkanlar",3),
            Tuple.Create("BAGLANTILAR", "Bağlantılar",4)
        };

        public static void Install(IServiceProvider provider)
        {

            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryLanguage = provider.GetService<IRepository<Language>>();
            var languages = repositoryLanguage.Get().Where(x => x.IsApproved).ToList();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var listPart = new List<Part>();
            var listPartHistory = new List<PartHistory>();

            var listPartLanguageLine = new List<PartLanguageLine>();
            var listPartLanguageLineHistory = new List<PartLanguageLineHistory>();

            var totalCount = PartTuples.Count * languages.Count;
            var counterPart = 1;

            foreach (var (item1, item2, item3) in PartTuples)
            {
                var item = new Part
                {
                    Id = GuidHelper.NewGuid(),
                    Code = item1,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModifier = user,
                    LastModificationTime = DateTime.Now
                };

                listPart.Add(item);
                var itemHistory = item.CreateMapped<Part, PartHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;

                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listPartHistory.Add(itemHistory);

                foreach (var language in languages)
                {
                    var line = new PartLanguageLine
                    {
                        Id = GuidHelper.NewGuid(),
                        Code = item1 + " " + language.Code,
                        Name = item2 + " " + language.Code,
                        DisplayOrder = item3,
                        Language = language,
                        Part = item,
                        CreationTime = DateTime.Now,
                        Creator = user,
                        LastModificationTime = DateTime.Now,
                        LastModifier = user,
                        Version = 1,
                        IsApproved = true
                    };

                    listPartLanguageLine.Add(line);

                    var lineHistory = line.CreateMapped<PartLanguageLine, PartLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = line.Id;
                    lineHistory.CreatorId = line.Creator.Id;
                    lineHistory.PartId = line.Part.Id;
                    lineHistory.LanguageId = line.Language.Id;
                    lineHistory.RestoreVersion = 0;
                    listPartLanguageLineHistory.Add(lineHistory);

                    Console.WriteLine(counterPart + @"/" + totalCount + @" PartLanguageLine (" + line.Code + @")");
                    counterPart++;

                }

            }


            unitOfWork.Context.AddRange(listPart);
            unitOfWork.Context.AddRange(listPartHistory);
            unitOfWork.Context.AddRange(listPartLanguageLine);
            unitOfWork.Context.AddRange(listPartLanguageLineHistory);
            unitOfWork.Context.SaveChanges();
        }

        public static void SetContents(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryPart = provider.GetService<IRepository<Part>>();
            var repositoryContent = provider.GetService<IRepository<Content>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");
            var repositoryPartContentLineList = new List<PartContentLine>();
            var repositoryPartContentLineHistoryList = new List<PartContentLineHistory>();

            var partContents = new List<Tuple<string, string>>
            {
                Tuple.Create("MANSET","1AE2A647-EE19-42EE-B737-A81800D52513"),
                Tuple.Create("MANSET","20DB39D5-E11F-42B9-9A00-A81800BBDE8A"),
                Tuple.Create("MANSET","42A02345-646B-49B2-98EE-A81800D2576D"),
                Tuple.Create("MANSET","8F8C46CC-7C58-4646-98F0-A81800D2640C"),
                Tuple.Create("MANSETALTI","9303CA32-5CDE-407C-AC89-A81800D524DA"),
                Tuple.Create("MANSETALTI","0CB38D13-DE6D-44DE-9A2E-A81800D26438"),
                Tuple.Create("MANSETALTI","8F8C46CC-7C58-4646-98F0-A81800D2640C"),
                Tuple.Create("ONECIKANLAR","0CB38D13-DE6D-44DE-9A2E-A81800D26438"),
                Tuple.Create("ONECIKANLAR","1AE2A647-EE19-42EE-B737-A81800D52513"),
                Tuple.Create("ONECIKANLAR","20DB39D5-E11F-42B9-9A00-A81800BBDE8A"),
                Tuple.Create("ONECIKANLAR","42A02345-646B-49B2-98EE-A81800D2576D"),
                Tuple.Create("ONECIKANLAR","8F8C46CC-7C58-4646-98F0-A81800D2640C"),
                Tuple.Create("ONECIKANLAR","9303CA32-5CDE-407C-AC89-A81800D524DA"),
                Tuple.Create("ONECIKANLAR","ac31b1a9-b441-4765-b706-6ec996d8cbe3"),
                Tuple.Create("ONECIKANLAR","deea1ddd-0c25-4ad7-be29-3b5bdc2a4e6a"),
                Tuple.Create("BAGLANTILAR","deea1ddd-0c25-4ad7-be29-3b5bdc2a4e6a"),



            };

            var counterPartContentLine = 1;
            var totalcounterPartContentLine = partContents.Count;
            foreach (var (item1, item2) in partContents)
            {
                var itemPart = repositoryPart.Get(x => x.Code == item1);
                var itemContent = repositoryContent.Get(x => x.Id == Guid.Parse(item2));

                var line = new PartContentLine
                {
                    Id = GuidHelper.NewGuid(),
                    Part = itemPart,
                    Content = itemContent,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModificationTime = DateTime.Now,
                    LastModifier = user,
                    Version = 1,
                    DisplayOrder = counterPartContentLine
                };
                repositoryPartContentLineList.Add(line);
                var lineHistory = line.CreateMapped<PartContentLine, PartContentLineHistory>();
                lineHistory.Id = GuidHelper.NewGuid();
                lineHistory.ReferenceId = line.Id;
                lineHistory.CreatorId = line.Creator.Id;
                lineHistory.PartId = line.Part.Id;
                lineHistory.ContentId = line.Content.Id;
                lineHistory.RestoreVersion = 0;
                repositoryPartContentLineHistoryList.Add(lineHistory);

                Console.WriteLine(counterPartContentLine + @"/" + totalcounterPartContentLine + @" PartContentLine (" + itemPart.Code + @" - " + itemContent.Code + @")");
                counterPartContentLine++;
            }

            unitOfWork.Context.AddRange(repositoryPartContentLineList);
            unitOfWork.Context.AddRange(repositoryPartContentLineHistoryList);
            unitOfWork.Context.SaveChanges();

        }
    }
}
