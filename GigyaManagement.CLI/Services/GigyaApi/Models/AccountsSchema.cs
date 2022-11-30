using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GigyaManagement.CLI.Services.GigyaApi.Models;

public sealed class AccountsSchema
{
    //[JsonPropertyName("profileSchema")]
    //public Schema ProfileSchema { get; set; }

    [JsonPropertyName("dataSchema")]
    public Schema DataSchema { get; set; }

    //[JsonPropertyName("subscriptionsSchema")]
    //public SubscriptionsSchema SubscriptionsSchema { get; set; }

    //[JsonPropertyName("preferencesSchema")]
    //public PreferencesSchema PreferencesSchema { get; set; }
}

public partial class Field
{
    [JsonPropertyName("required")]
    public bool SubscribeRequired { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    //[JsonPropertyName("allowNull")]
    //public bool AllowNull { get; set; }

    [JsonPropertyName("writeAccess")]
    public string WriteAccess { get; set; }

    [JsonPropertyName("encrypt"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Encrypt { get; set; }

    [JsonPropertyName("format"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Format { get; set; }
}

public partial class Schema
{
    [JsonPropertyName("fields")]
    public Dictionary<string, Field> Fields { get; set; }
}

public partial class PreferencesSchema
{
    [JsonPropertyName("fields")]
    public Dictionary<string, Preference> Fields { get; set; }
}

public partial class Preference
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("format")]
    public string Format { get; set; }

    [JsonPropertyName("required")]
    public bool TermsTosRequired { get; set; }

    [JsonPropertyName("writeAccess")]
    public string WriteAccess { get; set; }

    [JsonPropertyName("consentVaultRetentionPeriod")]
    public long ConsentVaultRetentionPeriod { get; set; }

    [JsonPropertyName("currentDocDate")]
    public DateTimeOffset CurrentDocDate { get; set; }

    [JsonPropertyName("minDocDate")]
    public DateTimeOffset MinDocDate { get; set; }
}

public partial class SubscriptionsSchema
{
    [JsonPropertyName("fields")]
    public Dictionary<string, Sub> Fields { get; set; }
}
public partial class Sub
{
    [JsonPropertyName("email")]
    public Email Email { get; set; }
}

public partial class Email
{
    [JsonPropertyName("required")]
    public bool EmailRequired { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("writeAccess")]
    public string WriteAccess { get; set; }

    [JsonPropertyName("doubleOptIn")]
    public bool DoubleOptIn { get; set; }

    [JsonPropertyName("description")]
    public object Description { get; set; }

    [JsonPropertyName("enableConditionalDoubleOptIn")]
    public bool EnableConditionalDoubleOptIn { get; set; }
}