using System;

namespace AnkaCMS.Core.Helpers
{
    /// <summary>
    /// Guid oluşturan sınıf
    /// </summary>
    public static class GuidHelper
    {
        private static readonly long BaseDateTicks = new DateTime(1900, 1, 1).Ticks;

        public static Guid NewGuid()
        {
            var guidArray = Guid.NewGuid().ToByteArray();
            var now = DateTime.UtcNow;
            var days = new TimeSpan(now.Ticks - BaseDateTicks);
            var msecs = now.TimeOfDay;
            var daysArray = BitConverter.GetBytes(days.Days);
            var msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
            return new Guid(guidArray);
        }

    }
}
