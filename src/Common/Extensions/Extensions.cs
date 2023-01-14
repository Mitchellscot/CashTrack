using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace CashTrack.Common.Extensions
{
    public static class Extensions
    {
        public static void AddToModelState(this ValidationResult result, ModelStateDictionary modelState)
        {
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    modelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
            }
        }
        public static int[] Accumulate(this int[] array)
        {
            var values = new int[array.Length];
            for (int i = 0; i <= array.Length - 1; i++)
            {
                if (i == 0)
                {
                    values[i] = array[i];
                    continue;
                }


                values[i] = values[i - 1] + array[i];
            }
            return values;
        }
        public static decimal[] Accumulate(this decimal[] array)
        {
            var values = new decimal[array.Length];
            for (int i = 0; i <= array.Length - 1; i++)
            {
                if (i == 0)
                {
                    values[i] = array[i];
                    continue;
                }


                values[i] = values[i - 1] + array[i];
            }
            return values;
        }
        //NOTE: This extension method does not work with EF Core and does not translate to SQL. Use .Example == "Example" instead when writing predicates for EF Core Queries 
        public static bool IsEqualTo(this string s, string compareTo) =>
            s.Equals(compareTo, System.StringComparison.CurrentCultureIgnoreCase);

        public static int ToPercentage(this int i, int total) =>
            total != 0 ? (int)decimal.Round(Convert.ToDecimal(i) / Convert.ToDecimal(total) * 100, 0) : 0;
        public static int ToPercentage(this decimal i, decimal total) =>
            total != 0 ? (int)decimal.Round(i / total * 100, 0) : 0;
        public static decimal ToDecimalPercentage(this decimal i, decimal total) =>
            total != 0 ? decimal.Round(i / total * 100, 1) : 0;
        public static decimal ToDecimalPercentage(this int i, int total) =>
            total != 0 ? decimal.Round(Convert.ToDecimal(i) / Convert.ToDecimal(total) * 100, 1) : 0;
    }
}
