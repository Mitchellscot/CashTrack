using System.ComponentModel.DataAnnotations;

namespace CashTrack.Models.Common
{
    public enum QueryOptions
    {
        Date,
        
        Amount,
        Merchant,
        [Display(Name="Sub Category")]
        SubCategory,
        [Display(Name = "Main Category")]
        MainCategory,
        Tag
    }
}

