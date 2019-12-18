using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnkaCMS.Data.DataAccess.EntityFramework;

namespace AnkaCMS.SetupConsoleApp.Installation
{
    public static class ContentInstallation
    {
        public static List<Tuple<string, string, int>> ContentTuples = new List<Tuple<string, string, int>>
        {

            Tuple.Create("KURUMSAL","0CB38D13-DE6D-44DE-9A2E-A81800D26438",1),
            Tuple.Create("HABERLER","1AE2A647-EE19-42EE-B737-A81800D52513",2),
            Tuple.Create("HABERLER","20DB39D5-E11F-42B9-9A00-A81800BBDE8A",3),
            Tuple.Create("HABERLER","42A02345-646B-49B2-98EE-A81800D2576D",4),
            Tuple.Create("BAGLANTILAR","8F8C46CC-7C58-4646-98F0-A81800D2640C",5),
            Tuple.Create("BAGLANTILAR","9303CA32-5CDE-407C-AC89-A81800D524DA",6),
            Tuple.Create("BAGLANTILAR","ac31b1a9-b441-4765-b706-6ec996d8cbe3",7),
            Tuple.Create("BAGLANTILAR","deea1ddd-0c25-4ad7-be29-3b5bdc2a4e6a",8),
        };

        public static void Install(IServiceProvider provider)
        {

            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var repositoryLanguage = provider.GetService<IRepository<Language>>();
            var repositoryCategory = provider.GetService<IRepository<Category>>();
            var languages = repositoryLanguage.Get().Where(x => x.IsApproved).ToList();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var listContent = new List<Content>();
            var listContentHistory = new List<ContentHistory>();
            var listContentLanguageLine = new List<ContentLanguageLine>();
            var listContentLanguageLineHistory = new List<ContentLanguageLineHistory>();

            var setupProjectRootPath = AppContext.BaseDirectory;
            if (AppContext.BaseDirectory.Contains("bin"))
            {
                setupProjectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
            }


            var setupProjectContentFiles = Path.Combine(setupProjectRootPath, "ContentFiles");

            //foreach (var directory in Directory.GetDirectories(setupProjectContentFiles))
            //{
            //    if (directory.Contains("Public")) continue;
            //    var directoryInfo = new DirectoryInfo(directory);
            //    foreach (var file in directoryInfo.GetFiles())
            //    {
            //        file.Delete();
            //    }
            //    foreach (var dir in directoryInfo.GetDirectories())
            //    {
            //        dir.Delete(true);
            //    }
            //    Directory.Delete(directory);
            //}

            var apiProjectContentFiles = setupProjectContentFiles.Replace("AnkaCMS.SetupConsoleApp", setupProjectContentFiles.Contains("\\") ? "AnkaCMS.WebApi\\wwwroot" : "AnkaCMS.WebApi/wwwroot");

            foreach (var directory in Directory.GetDirectories(apiProjectContentFiles))
            {
                if (directory.Contains("Public")) continue;
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


            var setupProjectPublicContentFiles = Path.Combine(setupProjectContentFiles, "Public");

            var totalCount = ContentTuples.Count * languages.Count;
            var counterContent = 1;

            foreach (var (item1, item2, item3) in ContentTuples)
            {
                var parent = repositoryCategory.Get(x => x.Code == item1);

                var item = new Content
                {
                    Id = Guid.Parse(item2),
                    Code = "yazi-ornegi-" + item3,
                    CreationTime = DateTime.Now,
                    Creator = user,
                    LastModifier = user,
                    LastModificationTime = DateTime.Now,
                    Category = parent
                };

                listContent.Add(item);
                var itemHistory = item.CreateMapped<Content, ContentHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.CategoryId = item.Category.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listContentHistory.Add(itemHistory);

                var contentDirectoryPath = Path.Combine(setupProjectContentFiles, item.Id.ToString());
                if (!Directory.Exists(contentDirectoryPath))
                {
                    Directory.CreateDirectory(contentDirectoryPath);
                    FileHelper.CopyDirectory(setupProjectPublicContentFiles, contentDirectoryPath);
                }

                foreach (var language in languages)
                {
                    var line = new ContentLanguageLine
                    {
                        Id = GuidHelper.NewGuid(),
                        Code = "yazi-ornegi-" + item3 + "-" + language.Code,
                        Name = "Yazı Örneği " + item3 + " Uzun Başlık " + language.Code,
                        ShortName = "Yazı Örneği " + item3 + " Kısa Başlık " + language.Code,
                        Description = "Yazı Örneği " + item3 + " Açıklama " + language.Code+ " bu alana gelecek. Yazı Örneği " + item3 + " Açıklama " + language.Code + " bu alana gelecek. Yazı Örneği " + item3 + " Açıklama " + language.Code + " bu alana gelecek. Yazı Örneği " + item3 + " Açıklama " + language.Code + " bu alana gelecek. Yazı Örneği " + item3 + " Açıklama " + language.Code + " bu alana gelecek. ",
                        Keywords = "Yazı Örneği " + item3 + " Anahtar Kelimeler " + language.Code,
                        ContentDetail = "Yazı Örneği " + item3 + " Detay Metni " + language.Code,
                        ImageName = "yazi-ornegi-" + item3 + "-" + language.Code + ".jpg",
                        ImageFileLength = 10,
                        ImageFileType = "image/jpeg",
                        ImagePath = "/ContentFiles/" + (item2 + "/yazi-ornegi-" + item3 + "-" + language.Code).ToLower() + ".jpg",
                        DisplayOrder = item3,
                        Language = language,
                        Content = item,
                        CreationTime = DateTime.Now,
                        Creator = user,
                        LastModificationTime = DateTime.Now,
                        LastModifier = user,
                        Version = 1,
                        IsApproved = true
                    };

                    listContentLanguageLine.Add(line);

                    var lineHistory = line.CreateMapped<ContentLanguageLine, ContentLanguageLineHistory>();
                    lineHistory.Id = GuidHelper.NewGuid();
                    lineHistory.ReferenceId = line.Id;
                    lineHistory.CreatorId = line.Creator.Id;
                    lineHistory.ReferenceId = line.Content.Id;
                    lineHistory.LanguageId = line.Language.Id;
                    lineHistory.ContentId = line.Content.Id;
                    lineHistory.RestoreVersion = 0;
                    listContentLanguageLineHistory.Add(lineHistory);

                    Console.WriteLine(counterContent + @"/" + totalCount + @" ContentLanguageLine (" + line.Code + @")");
                    counterContent++;
                }
            }

            
            FileHelper.CopyDirectory(setupProjectContentFiles, apiProjectContentFiles);

            unitOfWork.Context.AddRange(listContent);
            unitOfWork.Context.AddRange(listContentHistory);
            unitOfWork.Context.AddRange(listContentLanguageLine);
            unitOfWork.Context.AddRange(listContentLanguageLineHistory);
            unitOfWork.Context.SaveChanges();

        }
    }
}