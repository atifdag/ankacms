using AnkaCMS.Data.BaseEntities;

namespace AnkaCMS.Data.DataEntities
{

    public class PermissionHistory : BaseHistoryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}