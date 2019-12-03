using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using AnkaCMS.Core.ValueObjects;
using System.Collections.Generic;

namespace AnkaCMS.Service
{
    public interface IParameterGroupService : ICrudService<ParameterGroupModel>
    {
        List<IdCodeName> List();
    }
}
