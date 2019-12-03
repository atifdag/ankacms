using AnkaCMS.Data.BaseEntities;
using System.Collections.Generic;

namespace AnkaCMS.Data.DataEntities
{
    /// <inheritdoc />
    /// <summary>
    /// Api Resource
    /// </summary>
    public class Permission : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public virtual ICollection<RolePermissionLine> RolePermissionLines { get; set; }
        public virtual ICollection<PermissionMenuLine> PermissionMenuLines { get; set; }

    }
}