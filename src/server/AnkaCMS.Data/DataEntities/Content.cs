using AnkaCMS.Data.BaseEntities;
using System.Collections.Generic;

namespace AnkaCMS.Data.DataEntities
{
    public class Content : LanguageBaseEntity
    {
        public virtual Category Category { get; set; }
        public virtual ICollection<PartContentLine> PartContentLines { get; set; }
        public virtual ICollection<ContentLanguageLine> ContentLanguageLines { get; set; }
    }
}
