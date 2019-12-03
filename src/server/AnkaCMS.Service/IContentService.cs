using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface IContentService : ICrudService<ContentModel>
    {
        PublicContentModel PublicDetail(string code);
        ListModel<ContentModel> List(FilterModelWithLanguageAndParent filterModel);
        DetailModel<ContentModel> Detail(Guid contentId, Guid languageId);
        UpdateModel<ContentModel> Update(Guid contentId, Guid languageId);
        void Delete(Guid contentId, Guid languageId);
        List<IdCodeName> List(Guid languageId);
        ListModel<ContentModel> MyContentList(FilterModel filterModel);
        DetailModel<ContentModel> MyContentDetail(Guid contentId, Guid languageId);
        AddModel<ContentModel> MyContentAdd();
        AddModel<ContentModel> MyContentAdd(AddModel<ContentModel> addModel);
        UpdateModel<ContentModel> MyContentUpdate(Guid contentId, Guid languageId);
        UpdateModel<ContentModel> MyContentUpdate(UpdateModel<ContentModel> updateModel);
        void MyContentDelete(Guid contentId, Guid languageId);
    }
}
