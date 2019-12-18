using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface IPartService : ICrudService<PartModel>
    {
        ListModel<PartModel> List(FilterModelWithLanguage filterModel);
        DetailModel<PartModel> Detail(Guid partId, Guid languageId);
        UpdateModel<PartModel> Update(Guid partId, Guid languageId);
        void Delete(Guid partId, Guid languageId);
        List<IdCodeName> List(Guid languageId);
        PublicPartModel GetPublicPartContents(string partCode, string languageCode);
    }
}
