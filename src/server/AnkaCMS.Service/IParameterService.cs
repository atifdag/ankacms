using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.CrudBaseModels;

namespace AnkaCMS.Service
{
    public interface IParameterService : ICrudService<ParameterModel>
    {
        ListModel<ParameterModel> List(FilterModelWithParent filterModel);

    }
}
