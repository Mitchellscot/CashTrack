using System.ComponentModel.DataAnnotations;

namespace CashTrack.Models.Common
{
    public enum QueryOptions
    {
        Date,
        [Display(Name = "Date Range")]
        DateRange,
        [Display(Name = "Specific Month")]
        Month,
        [Display(Name = "Specific Quarter")]
        Quarter,
        [Display(Name = "Specific Year")]
        Year,
        [Display(Name = "Current Month")]
        CurrentMonth,
        [Display(Name = "Current Quarter")]
        CurrentQuarter,
        [Display(Name = "Current Year")]
        CurrentYear,
        [Display(Name = "Last 30 Days")]
        Last30Days,
        Amount,
        Notes,
        Merchant,
        [Display(Name = "Sub Category")]
        SubCategory,
        [Display(Name = "Main Category")]
        MainCategory,
        Tag
    }
}