using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.Context;

public class WorkspaceContext
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("userkey")]
    public string UserKey { get; set; }

    [JsonPropertyName("secret")]
    public string Secret { get; set; }

    [JsonPropertyName("workspace")]
    public string Workspace { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".gigman_workspaces/");
}
