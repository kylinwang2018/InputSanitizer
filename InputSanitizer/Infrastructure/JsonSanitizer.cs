using Ganss.Xss;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InputSanitizer.Infrastructure
{
    internal class JsonSanitizer
    {
        public static JsonElement Sanitize(
            JsonElement json,
            ModelStateDictionary modelState,
            string policyName = null,
            string lastPropertyName = null)
        {
            switch (json.ValueKind)
            {
                case JsonValueKind.Undefined:
                    // do nothing
                    break;
                case JsonValueKind.Object:
                    // 
                    var objNode = JsonNode.Parse(json.GetRawText());
                    foreach (var prop in objNode.AsObject().ToList())
                    {
                        var result = Sanitize(
                            JsonSerializer.SerializeToElement(prop.Value),
                            modelState, 
                            policyName, 
                            prop.Key);
                        objNode[prop.Key] = JsonNode.Parse(result.GetRawText());
                    }
                    json = JsonSerializer.SerializeToElement(objNode);
                    break;
                case JsonValueKind.Array:
                    // 
                    var arrayNode = JsonNode.Parse(json.GetRawText());

                    for (int i = 0; i < arrayNode.AsArray().Count; i++)
                    {
                        var result = Sanitize(
                           JsonSerializer.SerializeToElement(arrayNode[i]),
                           modelState,
                           policyName,
                           lastPropertyName);
                        arrayNode[i] = JsonNode.Parse(result.GetRawText());
                    }

                    json = JsonSerializer.SerializeToElement(arrayNode);
                    break;
                case JsonValueKind.String:
                    var text = json.ToString();
                    text = Cleaner.CleanString(lastPropertyName ?? "User Input", text, policyName, modelState);
                    json = JsonSerializer.SerializeToElement(text);
                    break;
                case JsonValueKind.Number:
                case JsonValueKind.True:
                case JsonValueKind.False:
                case JsonValueKind.Null:
                    // do nothing
                    break;
                default:
                    break;
            }
            return json;
        }
    }
}
