using System.Collections.Generic;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service.Models
{
    public class PublicContentFilterModel
    {
        public string Searched { get; set; }
        public Paging Paging { get; set; }
        public List<PublicContentModel> Contents { get; set; }
        public string Message { get; set; }

    }
}
