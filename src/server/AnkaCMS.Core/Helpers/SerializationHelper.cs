using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AnkaCMS.Core.Helpers
{
    public static class SerializationHelper
    {
        public static T DeserializeFromString<T>(string data)
        {
            var b = Convert.FromBase64String(data);
            using var stream = new MemoryStream(b);
            var formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
        public static string SerializeToString<T>(T data)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Flush();
            stream.Position = 0;
            return Convert.ToBase64String(stream.ToArray());
        }
    }
}
