using Newtonsoft.Json;

public class Response {
    [JsonProperty("_pages")]
    public int Pages { get; set; }

    [JsonProperty("emoticons")]
    public Emote[] Emotes { get; set; }
}