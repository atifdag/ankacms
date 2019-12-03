using System.IO;

namespace AnkaCMS.Core.ValueObjects
{
    public struct EmailAttachment
    {
        public Stream ContentStream { get; set; }
        public string Name { get; set; }
        public string MediaType { get; set; }
    }
}