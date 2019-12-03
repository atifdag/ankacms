using AnkaCMS.Data.BaseEntities;
using System.Collections.Generic;

namespace AnkaCMS.Data.DataEntities
{
    public class Category : LanguageBaseEntity
    {
        public virtual ICollection<Content> Contents { get; set; }
        public virtual ICollection<CategoryLanguageLine> CategoryLanguageLines { get; set; }
    }
}
