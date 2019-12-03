using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;
using AnkaCMS.Core.ValueObjects;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface IMenuService : ICrudService<MenuModel>
    {
        List<IdCodeName> List();
        ListModel<MenuModel> List(FilterModelWithParent filterModel);
    }
}
