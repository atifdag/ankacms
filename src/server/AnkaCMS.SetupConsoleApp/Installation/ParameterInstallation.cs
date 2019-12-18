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
    public static class ParameterInstallation
    {
        private static List<KeyValuePair<string, string>> ParameterGroups = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("PGMAIL", "Eposta Parametreleri"),
            new KeyValuePair<string, string>("PGAPPLICATION", "Uygulama Parametreleri"),
        };

        private static readonly List<Tuple<string, string, string>> Parameters = new List<Tuple<string, string, string>>
        {
            new Tuple<string, string, string>("PGAPPLICATION","DefaultLanguage","tr-TR"),
            new Tuple<string, string, string>("PGAPPLICATION","ApplicationName","AnkaCMS"),
            new Tuple<string, string, string>("PGAPPLICATION","ApplicationUrl","http://www.ankacms.com.tr"),
            new Tuple<string, string, string>("PGAPPLICATION","CorporationName","AnkaCMS"),
            new Tuple<string, string, string>("PGAPPLICATION","TaxAdministration","Çankaya V.D."),
            new Tuple<string, string, string>("PGAPPLICATION","TaxNumber","1234 5678 901"),
            new Tuple<string, string, string>("PGAPPLICATION","Address","Ankara"),
            new Tuple<string, string, string>("PGAPPLICATION","Phone","(312) 000 00 00"),
            new Tuple<string, string, string>("PGAPPLICATION","Fax","(312) 000 00 00"),
            new Tuple<string, string, string>("PGAPPLICATION","SendMailAfterUpdateUserInformation","true"),
            new Tuple<string, string, string>("PGAPPLICATION","SendMailAfterUpdateUserPassword","true"),
            new Tuple<string, string, string>("PGAPPLICATION","SendMailAfterAddUser","true"),
            new Tuple<string, string, string>("PGAPPLICATION","SessionTimeOut","20"),
            new Tuple<string, string, string>("PGAPPLICATION","PageSizeList","5,10,25,50,100,500"),
            new Tuple<string, string, string>("PGAPPLICATION","DefaultPageSize","10"),
            new Tuple<string, string, string>("PGAPPLICATION","EmailTemplatePath","wwwroot/EmailTemplates"),
            new Tuple<string, string, string>("PGAPPLICATION","MemoryCacheMainKey","AnkaCMSWebApiCacheMainKey"),
            new Tuple<string, string, string>("PGMAIL","SmtpServer","smtp.office365.com"),
            new Tuple<string, string, string>("PGMAIL","SmtpPort","587"),
            new Tuple<string, string, string>("PGMAIL","SmtpSsl","true"),
            new Tuple<string, string, string>("PGMAIL","SmtpUser","web@ankacms.com.tr"),
            new Tuple<string, string, string>("PGMAIL","SmtpPassword",""),
            new Tuple<string, string, string>("PGMAIL","SmtpSenderName","Anka CMS"),
            new Tuple<string, string, string>("PGMAIL","SmtpSenderMail","web@ankacms.com.tr"),
            new Tuple<string, string, string>("PGMAIL","UseDefaultCredentials","false"),
            new Tuple<string, string, string>("PGMAIL","UseDefaultNetworkCredentials","false"),
        };

        public static void Install(IServiceProvider provider)
        {
            var unitOfWork = provider.GetService<IUnitOfWork<EfDbContext>>();
            var repositoryUser = provider.GetService<IRepository<User>>();
            var user = repositoryUser.Get(x => x.Username == "atif.dag");

            var listParameterGroup = new List<ParameterGroup>();
            var listParameterGroupHistory = new List<ParameterGroupHistory>();
            var listParameter = new List<Parameter>();
            var listParameterHistory = new List<ParameterHistory>();

            var counterParameterGroup = 1;

            foreach (var (key, value) in ParameterGroups)
            {

                var item = new ParameterGroup
                {
                    Id = GuidHelper.NewGuid(),
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterParameterGroup,
                    Version = 1,
                    IsApproved = true,
                    Code = key,
                    Name = value,
                    Description = value,
                    Creator = user,
                    LastModifier = user
                };

                var itemHistory = item.CreateMapped<ParameterGroup, ParameterGroupHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;
                counterParameterGroup++;
                listParameterGroup.Add(item);
                listParameterGroupHistory.Add(itemHistory);

            }

            unitOfWork.Context.AddRange(listParameterGroup);
            unitOfWork.Context.AddRange(listParameterGroupHistory);
            unitOfWork.Context.SaveChanges();

            var counterParameter = 1;
            var listParameterCount = Parameters.Count;

            foreach (var (item1, item2, item3) in Parameters)
            {
                var itemParameterGroup = unitOfWork.Context.Set<ParameterGroup>().FirstOrDefault(x => x.Code == item1);

                var item = new Parameter
                {
                    Id = GuidHelper.NewGuid(),
                    Key = item2,
                    Value = item3,
                    CreationTime = DateTime.Now,
                    LastModificationTime = DateTime.Now,
                    DisplayOrder = counterParameter,
                    Version = 1,
                    IsApproved = true,
                    Creator = user,
                    LastModifier = user,
                    ParameterGroup = itemParameterGroup
                };

                listParameter.Add(item);

                var itemHistory = item.CreateMapped<Parameter, ParameterHistory>();
                itemHistory.Id = GuidHelper.NewGuid();
                itemHistory.ReferenceId = item.Id;
                itemHistory.ParameterGroupId = item.ParameterGroup.Id;
                itemHistory.CreatorId = item.Creator.Id;
                itemHistory.IsDeleted = false;
                itemHistory.RestoreVersion = 0;

                listParameterHistory.Add(itemHistory);

                Console.WriteLine(counterParameter + @"/" + listParameterCount + @" Parameter (" + item.Key + @")");

                counterParameter++;

            }

            unitOfWork.Context.AddRange(listParameter);
            unitOfWork.Context.AddRange(listParameterHistory);
            unitOfWork.Context.SaveChanges();
        }
    }
}

