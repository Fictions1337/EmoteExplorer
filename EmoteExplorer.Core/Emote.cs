using System.Collections.Generic;
using Newtonsoft.Json;

public class Emote {
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("animated")]
    public Dictionary<string, string> Animated { get; set; }

    [JsonProperty("urls")]
    public Dictionary<string, string> Urls { get; set; }

    [JsonProperty("owner")]
    public Dictionary<string, string> Owner { get; set; }
    
    [JsonProperty("usage_count")]
    public int UsageCount { get; set; }
}