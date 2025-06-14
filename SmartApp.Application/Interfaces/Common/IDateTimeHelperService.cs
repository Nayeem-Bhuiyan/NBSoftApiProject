using SmartApp.Application.DTOs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApp.Application.Interfaces.Common
{
    public interface IDateTimeHelperService
    {
        List<DropdownItemDto<int>> GetYearDropdown(int startYear, int endYear, int selectedYear);
        List<DropdownItemDto<int>> GetMonthDropdown(int selectedMonth);

        DateTime GetFirstDayOfMonth(int year, int month);
        DateTime GetFirstDayOfMonth(DateTime date);
        DateTime GetLastDayOfMonth(int year, int month);
        DateTime GetLastDayOfMonth(DateTime date);

        int GetDayDifference(DateTime fromDate, DateTime toDate);
        int GetMonthDifference(DateTime fromDate, DateTime toDate);
        int GetYearDifference(DateTime fromDate, DateTime toDate);

        DateTime GetMaxDate(DateTime date1, DateTime date2);
        DateTime GetMinDate(DateTime date1, DateTime date2);

        DatePartsDto GetDateParts(DateTime date);
    }
}
