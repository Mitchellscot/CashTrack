using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace CashTrack.Common
{
    public class CustomValidationFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }


            //if (!context.ModelState.IsValid)
            //{
            //    var errors = context.ModelState.Values.Where(v => v.Errors.Count > 0)
            //            .SelectMany(v => v.Errors)
            //            .Select(v => v.ErrorMessage)
            //            .ToList();

            //    var responseObj = new
            //    {
            //        Code = 123456,
            //        Message = "One or more validation errors occurred.",
            //        Errors = errors
            //    };

            //    context.Result = new JsonResult(responseObj)
            //    {
            //        StatusCode = 200
            //    };
            //}
        }
    }
}
