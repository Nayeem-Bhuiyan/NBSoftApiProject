using SmartApp.Application.DTOs.Common;
using SmartApp.Application.Interfaces.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SmartApp.Infrastructure.Services.Common.DateTimeHelperService;

namespace SmartApp.Infrastructure.Services.Common
{
        public class DateTimeHelperService : IDateTimeHelperService
        {
            public List<DropdownItemDto<int>> GetYearDropdown(int startYear, int endYear, int selectedYear)
            {
                var years = new List<DropdownItemDto<int>>();
                for (int year = startYear; year <= endYear; year++)
                {
                    years.Add(new DropdownItemDto<int>
                    {
                        Value = year,
                        Text = year.ToString(),
                        Selected = (year == selectedYear)
                    });
                }
                return years;
            }

            public List<DropdownItemDto<int>> GetMonthDropdown(int selectedMonth)
            {
                var months = new List<DropdownItemDto<int>>();
                for (int i = 1; i <= 12; i++)
                {
                    months.Add(new DropdownItemDto<int>
                    {
                        Value = i,
                        Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i),
                        Selected = (i == selectedMonth)
                    });
                }
                return months;
            }

            public DateTime GetFirstDayOfMonth(int year, int month) =>
                new DateTime(year, month, 1);

            public DateTime GetFirstDayOfMonth(DateTime date) =>
                new DateTime(date.Year, date.Month, 1);

            public DateTime GetLastDayOfMonth(int year, int month) =>
                new DateTime(year, month, DateTime.DaysInMonth(year, month));

            public DateTime GetLastDayOfMonth(DateTime date) =>
                new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));

            public int GetDayDifference(DateTime fromDate, DateTime toDate) =>
                (toDate.Date - fromDate.Date).Days;

            public int GetMonthDifference(DateTime fromDate, DateTime toDate) =>
                Math.Abs((toDate.Year - fromDate.Year) * 12 + toDate.Month - fromDate.Month);

            public int GetYearDifference(DateTime fromDate, DateTime toDate) =>
                Math.Abs(toDate.Year - fromDate.Year);

            public DateTime GetMaxDate(DateTime date1, DateTime date2) =>
                date1 > date2 ? date1 : date2;

            public DateTime GetMinDate(DateTime date1, DateTime date2) =>
                date1 < date2 ? date1 : date2;

            public DatePartsDto GetDateParts(DateTime date)
            {
                return new DatePartsDto
                {
                    Year = date.Year,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month),
                    DayName = date.DayOfWeek.ToString()
                };
            }
        }
    
}
