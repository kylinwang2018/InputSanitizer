using InputSanitizer.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Text.Json;

namespace InputSanitizer
{
    /// <summary>
    ///     Sanitize all input's property under this class or method
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class SanitizeAllInputFilter : Attribute, IActionFilter
    {
        /// <summary>
        ///     Sanitize input with specific policy
        /// </summary>
        public string PolicyName { get; set; }

        /// <summary>
        ///     Do noting
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        /// <summary>
        ///     After model binding, the sanitizer starts
        /// </summary>
        /// <param name="context"></param>
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Result != null)
                return;

            var actArgs = context.ActionArguments;

            if (!context.ActionArguments.Any()) return;

            try
            {
                foreach (var arg in actArgs)
                {
                    var dto = arg.Value;
                    var type = dto.GetType();
                    if (type == typeof(JsonElement))
                    {
                        actArgs[arg.Key] = JsonSanitizer.Sanitize((JsonElement)dto, context.ModelState, PolicyName);
                    }
                    else
                        Sanitizer.Sanitize(dto, true, context.ModelState, PolicyName);
                }
            }
            catch (ProhabitedStringValueException ex)
            {
                context.Result = new BadRequestObjectResult(
                    new { message = ex.Message ?? "Invalid input" });
                return;
            }
        }
    }
}
