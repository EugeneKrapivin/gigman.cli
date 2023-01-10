using System;

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
        return _gigyaService.ExecuteGigyaApi<SiteConfig>("admin.getSiteConfig", new Dictionary<string, string> 
        { 
            ["apikey"] = apikey,
            ["includeSiteGroupConfig"] = "true"
        });
    }
}