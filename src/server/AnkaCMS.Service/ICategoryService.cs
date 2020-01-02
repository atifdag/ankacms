using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.ValueObjects;
using System;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface ICategoryService : ICrudService<CategoryModel>
    {
        ListModel<CategoryModel> List(FilterModelWithLanguage filterModel);
        DetailModel<CategoryModel> Detail(Guid categoryId, Guid languageId);
        UpdateModel<CategoryModel> Update(Guid categoryId, Guid languageId);
        void Delete(Guid categoryId, Guid languageId);
        List<IdCodeName> List(Guid languageId);
        List<IdCodeName> List();
        PublicCategoryModel PublicDetail(string code);
    }
}
