using GigyaManagement.CLI.Services.Template.ProjectModels;

using System.Text.Json.Nodes;

namespace GigyaManagement.CLI.Services.GigyaApi;

public interface IGigyaService
{
    Task<TResult> ExecuteGigyaApi<TResult>(
        string command,
        Dictionary<string, string>? queryParams = null,
        Dictionary<string, string>? body = null);

}
