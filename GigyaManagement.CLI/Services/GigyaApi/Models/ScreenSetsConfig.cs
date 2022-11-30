using GigyaManagement.CLI.Services.Template.ProjectModels.Resources;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.GigyaApi.Models;

public class ScreenSetsConfig
{
    [JsonPropertyName("screenSets")]
    public ScreenSet[] ScreenSets { get; set; }
}

public class ScreenSet
{
    [JsonPropertyName("screenSetID")]
    public string ScreenSetId { get; set; }

    [JsonPropertyName("html")]
    public string Html { get; set; }

    [JsonPropertyName("css")]
    public string Css { get; set; }

    [JsonPropertyName("javascript")]
    public string Javascript { get; set; }

    [JsonPropertyName("translations")]
    public Dictionary<string, Dictionary<string, string>> Translations { get; set; }

    [JsonPropertyName("rawTranslations")]
    public string RawTranslations { get; set; }

    [JsonPropertyName("compressionType")]
    public long CompressionType { get; set; }
}