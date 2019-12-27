using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace AnkaCMS.Core.Helpers
{

    /// <summary>
    /// Tür tip dönüşümleri için yardımcı sınıf
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Objeyi byte türüne çevirir. Boş bırakılması durumunda varsayılan değer "0" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static byte ToByte(this object obj, byte defaultValue = default(byte))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return !byte.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// Objeyi short türüne çevirir. Boş bırakılması durumunda varsayılan değer "0" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static short ToShort(this object obj, short defaultValue = default(short))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return !short.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// Objeyi int türüne çevirir. Boş bırakılması durumunda varsayılan değer "0" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static int ToInt(this object obj, int defaultValue = default(int))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return !int.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// Objeyi double türüne çevirir. Boş bırakılması durumunda varsayılan değer "0" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static double ToDouble(this object obj, double defaultValue = default(double))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return !double.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// Objeyi decimal türüne çevirir. Boş bırakılması durumunda varsayılan değer "0" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static decimal ToDecimal(this object obj, decimal defaultValue = default(decimal))
        {
            if (obj == null)
            {
                return defaultValue;
            }

            return !decimal.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// Objeyi DateTime türüne çevirir. Boş bırakılması durumunda varsayılan değer "1.01.0001 00:00:00" olarak kabul edilir. Hata durumunda varsayılan değer döndürülür.
        /// </summary>
        /// <param name="obj">Obje</param>
        /// <param name="defaultValue">Varsayılan Değer</param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object obj, DateTime defaultValue = default(DateTime))
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
            {
                return defaultValue;
            }

            return !DateTime.TryParse(obj.ToString(), out var result) ? defaultValue : result;
        }

        /// <summary>
        /// string değerin int olup olmadığını döndürür.
        /// </summary>
        /// <param name="input">string</param>
        /// <returns>bool</returns>
        public static bool IsNumeric(this string input)
        {
            return int.TryParse(input, out var result);
        }

        /// <summary>
        /// Şartları sağlayan string ifadeyi bool tipine çevirir.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string str)
        {
            string[] trueStrings = { "1", "y", "yes", "true", "evet", "on" };
            string[] falseStrings = { "0", "n", "no", "false", "hayır", "hayir", "off" };
            if (trueStrings.Contains(str, StringComparer.OrdinalIgnoreCase)) return true;
            if (falseStrings.Contains(str, StringComparer.OrdinalIgnoreCase)) return false;
            throw new InvalidCastException("Yalnızca şu ifadeler dönüştürülür: " + string.Join(", ", trueStrings) + " ve " + string.Join(", ", falseStrings));
        }

        /// <summary>
        /// string bir değeri objeye dönüştürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(this string data)
        {
            var bytes = Convert.FromBase64String(data);
            using var stream = new MemoryStream(bytes);
            var formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }

        /// <summary>
        /// objeyi stringe dönüştürür.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string SerializeToString<T>(this T data)
        {
            using var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);
            stream.Flush();
            stream.Position = 0;
            return Convert.ToBase64String(stream.ToArray());
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }



    }
}