using AnkaCMS.Data.BaseEntities;
using System.Collections.Generic;

namespace AnkaCMS.Data.DataEntities
{
    public class Language : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLines { get; set; }
        public virtual ICollection<PartLanguageLine> PartLanguageLines { get; set; }
        public virtual ICollection<ContentLanguageLine> ContentLanguageLines { get; set; }
    }
}
