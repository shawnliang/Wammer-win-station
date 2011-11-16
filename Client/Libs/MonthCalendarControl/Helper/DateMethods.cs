namespace CustomControls
{
   using System;
   using System.Collections.Generic;
   using System.Globalization;

   /// <summary>
   /// Class for calendar methods.
   /// </summary>
   internal static class DateMethods
   {
      /// <summary>
      /// Gets the days in month for the specified date.
      /// </summary>
      /// <param name="date">The <see cref="MonthCalendarDate"/> to get the days in month for.</param>
      /// <returns>The number of days in the month.</returns>
      public static int GetDaysInMonth(MonthCalendarDate date)
      {
         return GetDaysInMonth(date.Calendar, date.Year, date.Month);
      }

      /// <summary>
      /// Gets the days in month for the specified year and month using the specified <paramref name="cal"/>.
      /// </summary>
      /// <param name="cal">The <see cref="Calendar"/> to use.</param>
      /// <param name="year">The year value.</param>
      /// <param name="month">The month value.</param>
      /// <returns>The number of days in the month using the specified <see cref="Calendar"/>.</returns>
      public static int GetDaysInMonth(Calendar cal, int year, int month)
      {
         return (cal ?? CultureInfo.CurrentUICulture.Calendar).GetDaysInMonth(year, month);
      }

      /// <summary>
      /// Gets the week of the year for the specified date, culture and calendar.
      /// </summary>
      /// <param name="info">The <see cref="CultureInfo"/> to use.</param>
      /// <param name="cal">The <see cref="Calendar"/> to use.</param>
      /// <param name="date">The date value to get the week of year for.</param>
      /// <returns>The week of the year.</returns>
      public static int GetWeekOfYear(CultureInfo info, Calendar cal, DateTime date)
      {
         CultureInfo ci = info ?? CultureInfo.CurrentUICulture;
         Calendar c = cal ?? ci.Calendar;

         return c.GetWeekOfYear(date, ci.DateTimeFormat.CalendarWeekRule, ci.DateTimeFormat.FirstDayOfWeek);
      }

      /// <summary>
      /// Gets the abbreviated and day names, beginning with the first day provided by the <paramref name="provider"/>.
      /// </summary>
      /// <param name="provider">The <see cref="ICustomFormatProvider"/> to use.</param>
      /// <returns>The abbreviated and day names beginning with the first day.</returns>
      public static string[,] GetDayNames(ICustomFormatProvider provider)
      {
         List<string> abbDayNames = new List<string>(provider.AbbreviatedDayNames);
         List<string> shortestDayNames = new List<string>(provider.ShortestDayNames);
         List<string> dayNames = new List<string>(provider.DayNames);
         string firstDayName = provider.GetAbbreviatedDayName(provider.FirstDayOfWeek);

         int firstNameIndex = abbDayNames.IndexOf(firstDayName);

         string[,] names = new string[3, dayNames.Count];
         int j = 0;

         for (int i = firstNameIndex; i < abbDayNames.Count; i++, j++)
         {
            names[0, j] = dayNames[i];
            names[1, j] = abbDayNames[i];
            names[2, j] = shortestDayNames[i];
         }

         for (int i = 0; i < firstNameIndex; i++, j++)
         {
            names[0, j] = dayNames[i];
            names[1, j] = abbDayNames[i];
            names[2, j] = shortestDayNames[i];
         }

         return names;
      }

      /// <summary>
      /// Gets the abbreviated or day names, beginning with the first day of the week specified by the <paramref name="provider"/>.
      /// </summary>
      /// <param name="provider">The provider to use.</param>
      /// <param name="index">The index for the kind of name : 0 means full, 1 means abbreviated and 2 means shortest day names.</param>
      /// <returns>The (abbreviated) day names.</returns>
      public static string[] GetDayNames(ICustomFormatProvider provider, int index)
      {
         if (index < 0 || index > 2)
         {
            index = 0;
         }

         string[,] dayNames = GetDayNames(provider);

         string[] names = new string[dayNames.GetLength(1)];

         for (int i = 0; i < names.Length; i++)
         {
            names[i] = dayNames[index, i];
         }

         return names;
      }

      /// <summary>
      /// Gets from the specified <see cref="CalendarDayOfWeek"/> a generic list
      /// with the work days.
      /// </summary>
      /// <param name="nonWorkDays">The non working days.</param>
      /// <returns>A list with the working days.</returns>
      public static List<DayOfWeek> GetWorkDays(CalendarDayOfWeek nonWorkDays)
      {
         List<DayOfWeek> workDays = new List<DayOfWeek>();

         for (int i = 0; i < 7; i++)
         {
            workDays.Add((DayOfWeek)i);
         }

         GetSysDaysOfWeek(nonWorkDays).ForEach(day =>
            {
               if (workDays.Contains(day))
               {
                  workDays.Remove(day);
               }
            });

         return workDays;
      }

      /// <summary>
      /// Gets a value indicating whether the specified culture is a right to left culture.
      /// </summary>
      /// <param name="info">The culture.</param>
      /// <returns>true, if RTL, false otherwise.</returns>
      public static bool IsRTLCulture(CultureInfo info)
      {
         return info.TextInfo.IsRightToLeft;
      }

      /// <summary>
      /// Gets a value indicating whether the current UI culture is a right to left culture.
      /// </summary>
      /// <returns>true, if RTL, false otherwise.</returns>
      public static bool IsRTLCulture()
      {
         return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;
      }

      /// <summary>
      /// Gets a generic list of <see cref="DayOfWeek"/> value from the
      /// specified <see cref="CalendarDayOfWeek"/> value.
      /// </summary>
      /// <param name="days">The <see cref="CalendarDayOfWeek"/> value.</param>
      /// <returns>A generic list of type <see cref="DayOfWeek"/>.</returns>
      private static List<DayOfWeek> GetSysDaysOfWeek(CalendarDayOfWeek days)
      {
         List<DayOfWeek> list = new List<DayOfWeek>();

         if ((days & CalendarDayOfWeek.Monday) == CalendarDayOfWeek.Monday)
         {
            list.Add(DayOfWeek.Monday);
         }

         if ((days & CalendarDayOfWeek.Tuesday) == CalendarDayOfWeek.Tuesday)
         {
            list.Add(DayOfWeek.Tuesday);
         }

         if ((days & CalendarDayOfWeek.Wednesday) == CalendarDayOfWeek.Wednesday)
         {
            list.Add(DayOfWeek.Wednesday);
         }

         if ((days & CalendarDayOfWeek.Thursday) == CalendarDayOfWeek.Thursday)
         {
            list.Add(DayOfWeek.Thursday);
         }

         if ((days & CalendarDayOfWeek.Friday) == CalendarDayOfWeek.Friday)
         {
            list.Add(DayOfWeek.Friday);
         }

         if ((days & CalendarDayOfWeek.Saturday) == CalendarDayOfWeek.Saturday)
         {
            list.Add(DayOfWeek.Saturday);
         }

         if ((days & CalendarDayOfWeek.Sunday) == CalendarDayOfWeek.Sunday)
         {
            list.Add(DayOfWeek.Sunday);
         }

         return list;
      }
   }
}