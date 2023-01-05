using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.GigyaApi.Models;

public partial class SiteConfig
{
    [JsonPropertyName("baseDomain")]
    public string BaseDomain { get; set; }

    [JsonPropertyName("dataCenter")]
    public string DataCenter { get; set; }

    [JsonPropertyName("trustedSiteURLs")]
    public string[] TrustedSiteUrLs { get; set; }

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("captchaProvider")]
    public string CaptchaProvider { get; set; }

    [JsonPropertyName("settings")]
    public SiteSettings? Settings { get; set; }

    [JsonPropertyName("trustedShareURLs")]
    public string[] TrustedShareUrLs { get; set; }

    [JsonPropertyName("enableDataSharing")]
    public bool EnableDataSharing { get; set; }

    [JsonPropertyName("isCDP")]
    public bool IsCdp { get; set; }

    [JsonPropertyName("invisibleRecaptcha")]
    public InvisibleRecaptcha? InvisibleRecaptchaConfig { get; set; }

    [JsonPropertyName("recaptchaV2")]
    public InvisibleRecaptcha? RecaptchaV2 { get; set; }

    [JsonPropertyName("funCaptcha")]
    public FunCaptcha? FunCaptchaConfig { get; set; }

    [JsonPropertyName("customAPIDomainPrefix")]
    public string CustomApiDomainPrefix { get; set; }

    public partial class FunCaptcha
    {
    }

    public partial class InvisibleRecaptcha
    {
        [JsonPropertyName("SiteKey")]
        public string SiteKey { get; set; }

        [JsonPropertyName("Secret")]
        public string Secret { get; set; }
    }

    public partial class SiteSettings
    {
        [JsonPropertyName("CNAME")]
        public string Cname { get; set; }

        [JsonPropertyName("shortURLDomain")]
        public string ShortUrlDomain { get; set; }

        [JsonPropertyName("shortURLRedirMethod")]
        public string ShortUrlRedirMethod { get; set; }

        [JsonPropertyName("encryptPII")]
        public bool EncryptPii { get; set; }
    }
}
