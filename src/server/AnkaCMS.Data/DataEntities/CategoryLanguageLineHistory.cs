using AnkaCMS.Data.BaseEntities;
using System;

namespace AnkaCMS.Data.DataEntities
{
    public class CategoryLanguageLineHistory : LanguageLineBaseHistoryEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Keywords { get; set; }
        public Guid CategoryId { get; set; }
    }
}
