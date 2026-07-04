using System.Text.Json.Serialization;

namespace MetanetA_MobileApp.Model;

public class JobOfferApiItem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("requiredJob")]
    public string RequiredJob { get; set; } = string.Empty;

    [JsonPropertyName("seniority")]
    public string Seniority { get; set; } = string.Empty;

    [JsonPropertyName("skills")]
    public string Skills { get; set; } = string.Empty;

    [JsonPropertyName("skillsWeight")]
    public int SkillsWeight { get; set; }
}
