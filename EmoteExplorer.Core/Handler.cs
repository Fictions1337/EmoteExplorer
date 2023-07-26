using System.IO;
using TextCopy;
using System.Linq;
using System;
using System.Collections.Generic;

public class JsonObject {
    public string Html { get; set; }
    public bool StatusOk { get; set; }
    public string Query { get; set; }
    public string BaseUrl { get; set; }
    public bool IsDark { get; set; }
    public int TotalPages { get; set; }
    public string Size { get; set; }
}

public class Handler {
    public JsonObject GetEmotes(JsonObject request) {
        var obj = new JsonObject();

        // Create a new ApiClient object
        var api = new ApiClient();

        // Check if the baseUrl is set
        if (request.BaseUrl != null)
            api.BaseUrl = request.BaseUrl;
        else
            api.BaseUrl = "https://api.frankerfacez.com/v1/emotes";
        
        // Set the query
        api.Query = request.Query;

        // Send a GET request
        api.GetEmotes();
        obj.StatusOk = api.StatusOk;

        // Check if the request succeeded
        if (!obj.StatusOk) {
            // Handle the error
            obj.Html = api.Error;

            // Return the object
            return obj;
        }

        // Set the total pages count
        obj.TotalPages = api.Pages;
        
        // Generate the HTML
        obj.Html = GenerateHtml(api.Emotes, request.Size);

        // Return the object
        return obj;
    }

    public void CopyLink(JsonObject request) {
        ClipboardService.SetText(request.Html);
    }

    public JsonObject SaveEmote(JsonObject request) {
        var obj = new JsonObject();

        // Get the emote link, name and exxtension
        var link = request.Html.Split(",")[0];
        var ext = link.Contains("animated") ? ".gif" : ".png";
        var name = request.Html.Split(",")[1];

        // Get Emote data
        var client = new ApiClient
        {
            BaseUrl = link
        };

        // Recieve the emote data (byte array)
        var data = client.GetEmote();

        // Check if the request succeeded
        if (data == null) {
            // Handle the error
            obj.StatusOk = false;
            obj.Html = client.Error;

            // Return the object
            return obj;
        }

        // EmoteExplorer folder path
        var defaultPath = Path.Combine(System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.MyPictures), "EmoteExplorer");
        
        // Check if the directory exists
        if (!Directory.Exists(defaultPath))
            // Create the directory
            Directory.CreateDirectory(defaultPath);

        // Image path
        var path = Path.Combine(defaultPath, name + ext);

        // Check if the file exists
        if (File.Exists(path)) {
            int i = 0;

            // Loop until the file doesn't exist
            while (true) {
                i++;
                if (File.Exists(path.Replace(name, name + "-" + i)))
                    continue;
                
                path = path.Replace(name, name + "-" + i);
                break;
            }
        }

        // Save the emote
        File.WriteAllBytes(path, data.Content.ReadAsByteArrayAsync().Result);

        // Set the status to true
        obj.StatusOk = true;

        // Return the object
        return obj;
    }

    public string GenerateHtml(Emote[] emotes, string size = "small") {
        // Read the HTML template
        using var reader = new StreamReader("App/emote.html");
        var html = reader.ReadToEnd();
        
        string proccessedHtml = "";
        int i = 0;

        // Loop through the emotes
        foreach (var emote in emotes) {
            // Increment the index
            i++;

            string  url;

            // Check if the emote is animated
            if (emote.Animated != null) {
                url = GetSize(emote.Animated, size);
            }
            else
                url = GetSize(emote.Urls, size);

            // Replace the placeholders with the emote data
            proccessedHtml += html.Replace("{name}", emote.Name)
                                   .Replace("{url}", url)
                                   .Replace("{owner}", emote.Owner["display_name"])
                                   .Replace("{count}", emote.UsageCount.ToString())
                                   .Replace("{emoteBox}", "emoteBox" + i)
                                   .Replace("{emote}", "emote" + i)
                                   .Replace("{copy}", "copy-" + i)
                                   .Replace("{save}", "save-" + i)
                                   .Replace("\"", "'");
        }

        // Return the HTML
        return proccessedHtml;
    }

    public JsonObject ThemeToggle(JsonObject request) {
        var obj = new JsonObject();

        // Get the theme
        var theme = request.IsDark ? "light" : "dark";

        // Write the theme to the config file
        File.WriteAllText("App/config.json", "{\"theme\": \""
                             + theme + "\"}");
        
        // Set the status
        obj.IsDark = !request.IsDark;

        // Return the object
        return obj;
    }

    public JsonObject LoadSettings() {
        var obj = new JsonObject();

        // Read the config file
        using var reader = new StreamReader("App/config.json");
        var config = reader.ReadToEnd();

        // Set the theme
        if (config.Contains("dark"))
            obj.IsDark = true;
        else
            obj.IsDark = false;

        // Return the object
        return obj;
    }

    static string GetSize(Dictionary<string, string> urls, string size) {
        // Get the size keys
        string[] sizesKeys = urls.Keys.ToArray();
        int[] ints = Array.ConvertAll(sizesKeys, int.Parse);

        // Check if the size is valid
        if (ints.Contains(1) && size == "small")
            return urls["1"];
        else if (ints.Contains(2) && size == "medium")
            return urls["2"];
        else if (ints.Contains(4) && size == "large")
            return urls["4"];
        else
            return urls["1"];
    }

    public void OpenEmoteDirectory() {
        // EmoteExplorer folder path
        var defaultPath = Path.Combine(System.Environment.GetFolderPath(
            System.Environment.SpecialFolder.MyPictures), "EmoteExplorer");

        // Check if the directory exists
        if (!Directory.Exists(defaultPath))
            // Create the directory
            Directory.CreateDirectory(defaultPath);

        // Open the directory
        System.Diagnostics.Process.Start("explorer.exe", defaultPath);
    }
}