using System;

namespace AnkaCMS.Core.Helpers
{

    /// <summary>
    /// Tarih ve zaman işlemleri için yardımcı sınıf
    /// </summary>
    public static class DateTimeHelper
    {
        /// <summary>
        /// Şimdiki ayın ilk günü
        /// </summary>
        /// <returns></returns>
        public static DateTime FirstOfCurrentMonth()
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0, 0);
        }

        /// <summary>
        /// Günün ilk zamanı
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>

        public static DateTime ResetTimeToStartOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Günün datetime türünden son zamanı
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ResetTimeToEndOfDay(this DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }
    }
}
