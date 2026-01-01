using System.Text.Json;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CardsAgainstHumanity.Api.Extensions;

public static class FunctionX
{
    public static async Task<T> ReadBodyAsync<T>(this HttpRequestData req) where T : class
    {
        var content = await new StreamReader(req.Body).ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public static async Task<T> ReadBodyParamAsync<T>(this HttpRequestData req, string property) where T : class
    {
        var content = await new StreamReader(req.Body).ReadToEndAsync();
        var response = JObject.Parse(content);
        return response[property]!.Value<T>()!;
    }

    public static string? GetRouteValue(this HttpRequestData req, string property)
    {
        if (req.FunctionContext.BindingContext.BindingData.TryGetValue(property, out var value))
        {
            return value?.ToString();
        }
        return null;
    }

    public static string? Query(this HttpRequestData req, string key)
    {
        var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
        return query[key];
    }
}
