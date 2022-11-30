using GigyaManagement.CLI.Services.GigyaApi.Models;

namespace GigyaManagement.CLI.Services.GigyaApi.Configurators;

public class ScreenSetsConfigurator : IGigyaResourceConfigurator<ScreenSetsConfig, string>
{
    private IGigyaService _gigyaService;

    public ScreenSetsConfigurator(IGigyaService gigyaService)
    {
        _gigyaService = gigyaService;
    }

    public async Task Apply(string apikey, ScreenSetsConfig resource)
    {
        await _gigyaService.ExecuteGigyaApi<GigyaResponse>("accounts.setScreenSets",
            new Dictionary<string, string> { ["apikey"] = apikey },
            resource.ToGigyaFormUrl());
    }

    public Task<ScreenSetsConfig> Extract(string apikey)
    {
        return _gigyaService.ExecuteGigyaApi<ScreenSetsConfig>("accounts.getScreenSets", new Dictionary<string, string> { ["apikey"] = apikey });
    }
}