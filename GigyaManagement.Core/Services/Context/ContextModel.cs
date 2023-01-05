using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Context;

public class ContextModel
{
    [JsonPropertyName("current_context")]
    public string CurrentContext { get; set; } = string.Empty;

    [JsonPropertyName("defined_context")]
    public List<WorkspaceContext> DefinedContexts { get; set; } = new();
}
