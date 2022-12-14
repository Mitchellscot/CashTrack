using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using System.Linq;

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
    }
}
