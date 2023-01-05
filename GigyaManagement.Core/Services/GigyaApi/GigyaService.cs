using GigyaManagement.CLI.Services.Context;
using GigyaManagement.CLI.Services.GigyaApi.Models;

using System.CodeDom.Compiler;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.GigyaApi;

public class GigyaService : IGigyaService, IDisposable
{
    private readonly WorkspaceContext _context;
    private readonly HttpClient _httpClient;
    private readonly bool _shouldDispose = true;
    private bool _disposedValue;

    public GigyaService(WorkspaceContext context)
    {
        _context = context;
        _httpClient = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = System.Net.DecompressionMethods.GZip
        });

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<TResult> ExecuteGigyaApi<TResult>(
        string command,  
        Dictionary<string, string>? queryParams = null, 
        Dictionary<string, string>? body = null)
    {
        var domain = $"https://{GetDomain(command)}";
        var baseQuery = $"userkey={_context.UserKey}&secret={_context.Secret}&httpStatusCodes=true";
        
        if (queryParams?.Any() == true)
        {
            var query = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}"));
            baseQuery = baseQuery + "&" + query;
        }

        HttpResponseMessage gigyaResponse = await Execute($"{domain}/{command}?{baseQuery}", body);

        if (!gigyaResponse.IsSuccessStatusCode)
        {
            if (gigyaResponse.StatusCode == System.Net.HttpStatusCode.MovedPermanently)
            {
                var bytes = await gigyaResponse.Content.ReadAsByteArrayAsync();
                var r = Encoding.UTF8.GetString(bytes);
                var gig = JsonSerializer.Deserialize<GigyaResponse>(r)!;
                gigyaResponse = await Execute($"{gig.Location}?{baseQuery}", body);
            }
            else 
            {
                var result = await gigyaResponse.Content.ReadAsStringAsync();
                throw new Exception(result);
            }
        }

        var siteConfig = await gigyaResponse.Content.ReadFromJsonAsync<TResult>();

        if (siteConfig is null)
        {
            throw new Exception("got a null result from gigya");
        }

        return siteConfig;

        Task<HttpResponseMessage> Execute(string url, Dictionary<string,string>? payload = null)
        {
            return payload?.Any() == true
                ? _httpClient.PostAsync($"{url}", new FormUrlEncodedContent(payload))
                : _httpClient.GetAsync($"{url}");
        }

        static string GetDomain(string command)
        {
            if (command.StartsWith("admin")) return "admin.us1.gigya.com";
            if (command.StartsWith("accounts")) return "accounts.us1.gigya.com";

            throw new NotSupportedException($"cant calculate the required gigya namespace for \"{command}\"");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_shouldDispose)
                {
                    _httpClient.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public sealed class GigyaCreateSiteResopnse : IGigyaResopnse
{
    public long SiteId { get; set; }
    public string ApiKey { get; set; }

    [JsonPropertyName("callId")]
    public string CallId { get; set; }

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("errorDetails")]
    public string ErrorDetails { get; set; }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }

    [JsonPropertyName("apiVersion")]
    public int ApiVersion { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}

public sealed class GigyaGetSiteConfig : SiteConfig, IGigyaResopnse
{
    [JsonPropertyName("callId")]
    public string CallId { get; set; }

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("errorDetails")]
    public string ErrorDetails { get; set; }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }

    [JsonPropertyName("apiVersion")]
    public int ApiVersion { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
}

public sealed class GigyaResponse : IGigyaResopnse
{
    [JsonPropertyName("callId")]
    public string CallId { get; set; }

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; set; }

    [JsonPropertyName("errorDetails")]
    public string ErrorDetails { get; set; }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get; set; }

    [JsonPropertyName("apiVersion")]
    public int ApiVersion { get; set; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; set; }

    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("location")]
    public string Location { get; set; }
}

public interface IGigyaResopnse
{
    [JsonPropertyName("callId")]
    public string CallId { get;  }

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; }

    [JsonPropertyName("errorDetails")]
    public string ErrorDetails { get;  }

    [JsonPropertyName("errorMessage")]
    public string ErrorMessage { get;}

    [JsonPropertyName("apiVersion")]
    public int ApiVersion { get; }

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; }

    [JsonPropertyName("statusReason")]
    public string StatusReason { get; }

    [JsonPropertyName("time")]
    public DateTime Time { get; }
}