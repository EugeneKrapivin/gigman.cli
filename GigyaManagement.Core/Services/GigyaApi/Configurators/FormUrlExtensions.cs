using System.Text.Json;
using System.Text.Json.Nodes;

namespace GigyaManagement.CLI.Services.GigyaApi.Configurators;

public static class FormUrlExtensions
{
    public static Dictionary<string,string> ToGigyaFormUrl<T>(this T source, params Func<string, bool>[] filters)
        => JsonNode
            .Parse(JsonSerializer.Serialize(source))!
            .AsObject()
            .Where(x => filters.All(filter => !filter(x.Key)))
            .Aggregate(new Dictionary<string,string>(), (acc, elem) =>
            {
                acc.Add(elem.Key, Serialize(elem.Value));
                return acc;
            });
    private static string Serialize(JsonNode jsonNode) => jsonNode switch
    {
        JsonObject x => x.ToJsonString(),
        JsonArray x => x.ToJsonString(),
        JsonValue x => x.ToString(),
        _ => throw new NotImplementedException()
    };
}