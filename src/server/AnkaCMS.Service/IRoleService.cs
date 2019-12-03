using AnkaCMS.Service.Models;
using AnkaCMS.Core;
using System;
using System.Collections.Generic;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service
{
    public interface IRoleService : ICrudService<RoleModel>
    {
        List<Guid> GetActionRoles(string controller, string action);

        List<IdCodeName> List();
    }
}
