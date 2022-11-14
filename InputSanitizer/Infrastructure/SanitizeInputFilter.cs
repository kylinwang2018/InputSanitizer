using Ganss.Xss;
using InputSanitizer.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace InputSanitizer
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SanitizeInputFilter : Attribute, IActionFilter
    {
        public string PolicyName { get; set; }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Result != null)
                return;

            var actArgs = context.ActionArguments;

            if (!context.ActionArguments.Any()) return;

            try
            {
                foreach (var dto in actArgs.Values)
                {
                    Sanitizer.Sanitize(dto, false, context.ModelState, PolicyName);
                }
            }
            catch(ProhabitedStringValueException ex)
            {
                context.Result = new BadRequestObjectResult(
                    new { message = ex.Message ?? "Invalid input" });
                return;
            }
        }
    }
}
