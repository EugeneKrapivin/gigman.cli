using GigyaManagement.CLI.Services.GigyaApi.Models;

using System.Text.Json;

using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.GigyaApi.Configurators;

public class GigyaSchemaConfigurator : IGigyaResourceConfigurator<AccountsSchema, string>
{
    private IGigyaService _gigyaService;

    public GigyaSchemaConfigurator(IGigyaService gigyaService)
    {
        _gigyaService = gigyaService;
    }

    public async Task Apply(string apikey, AccountsSchema resource)
    {
        if (resource is null) throw new ArgumentNullException(nameof(resource));

        //var cur = JsonObject.Parse(JsonSerializer.Serialize(await Extract(apikey)));
        //var loc = JsonObject.Parse(JsonSerializer.Serialize(resource));
        //var diff = loc.Diff(cur, new()
        //{
        //    JsonElementComparison = JsonElementComparison.Semantic,
        //});

        await _gigyaService.ExecuteGigyaApi<GigyaResponse>("accounts.setSchema", 
            new Dictionary<string, string> { ["apikey"] = apikey }, 
            new RequestModel { DataSchema = resource.DataSchema}.ToGigyaFormUrl(p => p == "dynamicSchema"));
    }
    private class RequestModel
    {
        [JsonPropertyName("dataSchema")]
        public Schema DataSchema { get; set; }
    }

    public Task<AccountsSchema> Extract(string apikey)
    {
        return _gigyaService.ExecuteGigyaApi<AccountsSchema>("accounts.getSchema", new Dictionary<string, string> { ["apikey"] = apikey });
    }
}
