using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CardsAgainstHumanity.Api.Extensions;

public static class FunctionX
{
    public static async Task<T> ReadBodyAsync<T>(this HttpRequest req) where T : class
    {
        var content = await new StreamReader(req.Body).ReadToEndAsync();
        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public static async Task<T> ReadBodyParamAsync<T>(this HttpRequest req, string property) where T : class
    {
        var content = await new StreamReader(req.Body).ReadToEndAsync();
        var response = JObject.Parse(content);
        return response[property]!.Value<T>()!;
    }
}
