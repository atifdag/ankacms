using AnkaCMS.Data.DataEntities;
using AnkaCMS.Core;
using AnkaCMS.Core.Helpers;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service.Implementations
{
    public class MainService : IMainService
    {
        private readonly IRepository<Parameter> _repositoryParameter;


        public MainService(IRepository<Parameter> repositoryParameter)
        {
            _repositoryParameter = repositoryParameter;
        }

        public ApplicationSettings ApplicationSettings => new ApplicationSettings
        {
            Address = _repositoryParameter.Get(x => x.Key == "Address").Value,
            ApplicationName = _repositoryParameter.Get(x => x.Key == "ApplicationName").Value,
            ApplicationUrl = _repositoryParameter.Get(x => x.Key == "ApplicationUrl").Value,
            CorporationName = _repositoryParameter.Get(x => x.Key == "CorporationName").Value,
            DefaultLanguage = _repositoryParameter.Get(x => x.Key == "DefaultLanguage").Value,
            DefaultPageSize = _repositoryParameter.Get(x => x.Key == "DefaultPageSize").Value.ToInt(),
            EmailTemplatePath = _repositoryParameter.Get(x => x.Key == "EmailTemplatePath").Value,
            Fax = _repositoryParameter.Get(x => x.Key == "Fax").Value,
            MemoryCacheMainKey = _repositoryParameter.Get(x => x.Key == "MemoryCacheMainKey").Value,
            PageSizeList = _repositoryParameter.Get(x => x.Key == "PageSizeList").Value,
            Phone = _repositoryParameter.Get(x => x.Key == "Phone").Value,
            SendMailAfterAddUser = _repositoryParameter.Get(x => x.Key == "SendMailAfterAddUser").Value.ToBoolean(),
            SendMailAfterUpdateUserInformation = _repositoryParameter.Get(x => x.Key == "SendMailAfterUpdateUserInformation").Value.ToBoolean(),
            SendMailAfterUpdateUserPassword = _repositoryParameter.Get(x => x.Key == "SendMailAfterUpdateUserPassword").Value.ToBoolean(),
            SessionTimeOut = _repositoryParameter.Get(x => x.Key == "SessionTimeOut").Value.ToInt(),
            SmtpPassword = _repositoryParameter.Get(x => x.Key == "SmtpPassword").Value,
            SmtpPort = _repositoryParameter.Get(x => x.Key == "SmtpPort").Value.ToInt(),
            SmtpSenderMail = _repositoryParameter.Get(x => x.Key == "SmtpSenderMail").Value,
            SmtpSenderName = _repositoryParameter.Get(x => x.Key == "SmtpSenderName").Value,
            SmtpServer = _repositoryParameter.Get(x => x.Key == "SmtpServer").Value,
            SmtpSsl = _repositoryParameter.Get(x => x.Key == "SmtpSsl").Value.ToBoolean(),
            SmtpUser = _repositoryParameter.Get(x => x.Key == "SmtpUser").Value,
            TaxAdministration = _repositoryParameter.Get(x => x.Key == "TaxAdministration").Value,
            TaxNumber = _repositoryParameter.Get(x => x.Key == "TaxNumber").Value,
            UseDefaultCredentials = _repositoryParameter.Get(x => x.Key == "UseDefaultCredentials").Value.ToBoolean(),
            UseDefaultNetworkCredentials = _repositoryParameter.Get(x => x.Key == "UseDefaultNetworkCredentials").Value.ToBoolean(),
        };
    }
}
