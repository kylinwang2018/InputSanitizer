# InputSanitizer

[![NuGet version](https://badge.fury.io/nu/inputsanitizer.svg)](https://badge.fury.io/nu/inputsanitizer)

InputSanitizer is a filter used to clear all illegal strings from the data received by the API or MVC Controller.

It is based on HtmlSanitizer as an HTML-related cleaner, and also adds Regex pattern matching cleanup.

It supports data model binding in DTO mode and also supports dynamic types based on Json.

## How To Use

Add InputSanitize to ServiceCollection with policy in Program.cs

```cs
var policy = new InputSanitizerPolicy()
            {
                Name = "Default",
                AllowedTags = new HashSet<string>(HtmlSanitizerDefaults.AllowedTags, StringComparer.OrdinalIgnoreCase),
                AllowedSchemes = new HashSet<string>(HtmlSanitizerDefaults.AllowedSchemes, StringComparer.OrdinalIgnoreCase),
                AllowedAttributes = new HashSet<string>(HtmlSanitizerDefaults.AllowedAttributes, StringComparer.OrdinalIgnoreCase),
                UriAttributes = new HashSet<string>(HtmlSanitizerDefaults.UriAttributes, StringComparer.OrdinalIgnoreCase),
                AllowedCssProperties = new HashSet<string>(HtmlSanitizerDefaults.AllowedCssProperties, StringComparer.OrdinalIgnoreCase),
                AllowedAtRules = new HashSet<CssRuleType>(HtmlSanitizerDefaults.AllowedAtRules),
                AllowedCssClasses = new HashSet<string>(HtmlSanitizerDefaults.AllowedClasses),
                ExceptionMessage = "What are you doing?",
                InvalidInputBehaviour = InvalidInputBehaviour.SetModelState
            }
            
builder.Services.AddInputSanitizer(InputSanitizerPolicies.Policies);
```

Add To the Controller

```cs
    [ApiController]
    [Route("[controller]")]
    [SanitizeAllInputFilter(PolicyName = "Default")]  // this line HERE
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "PostWeatherForecast")]
        public IActionResult Post(DTOModel data)    // OR public IActionResult Post(dynamic data)
        {
            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }
            return Ok(data);
        }
    }
```
