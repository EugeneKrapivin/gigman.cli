using System;
using System.Text.Json;
using System.Text.Json.JsonDiffPatch;
using System.Text.Json.JsonDiffPatch.Diffs.Formatters;
using System.Text.Json.Nodes;

using GigyaManagement.CLI.Services.GigyaApi.Models;

namespace GigyaManagement.CLI.Services.GigyaApi.Configurators;

public class GigyaSiteConfigConfigurator : IGigyaResourceConfigurator<SiteConfig, string>
{
    private readonly IGigyaService _gigyaService;

    public GigyaSiteConfigConfigurator(IGigyaService gigyaService)
    {
        _gigyaService = gigyaService;
    }

    public async Task Apply(string apikey, SiteConfig resource)
    {
        if (resource is null) throw new ArgumentNullException(nameof(resource));

        //var cur = JsonObject.Parse(JsonSerializer.Serialize(await Extract(apikey)));
        //var loc = JsonObject.Parse(JsonSerializer.Serialize(resource));
        //var diff = cur.Diff(loc, new()
        //{
        //    JsonElementComparison = JsonElementComparison.Semantic,
        //});

        await _gigyaService.ExecuteGigyaApi<GigyaResponse>("admin.setSiteConfig", 
            new Dictionary<string, string> { ["apikey"] = apikey },
            resource.ToGigyaFormUrl(
                p => p == "customAPIDomainPrefix",
                p => p == "isCDP"));
    }

    public Task<SiteConfig> Extract(string apikey)
    {
        return _gigyaService.ExecuteGigyaApi<SiteConfig>("admin.getSiteConfig", new Dictionary<string, string> { ["apikey"] = apikey });
    }
}

public static class FormUrlExtensions
{
    public static Dictionary<string,string> ToGigyaFormUrl<T>(this T source, params Func<string, bool>[] filters)
        => JsonNode.Parse(JsonSerializer.Serialize(source)).AsObject()
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