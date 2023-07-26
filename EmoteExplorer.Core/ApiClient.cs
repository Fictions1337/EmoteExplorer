using System;
using System.Net.Http;

public class ApiClient {

    // Properties
    public string BaseUrl { get; set; }
    public string Query { get; set; }
    public int Pages { get; set; }
    public Emote[] Emotes { get; set; }
    public bool StatusOk { get; set; }
    public string Error { get; set; }

    // GET request Handler
    public void GetEmotes() {

        // Create a new HttpClient object.
        using var client = new HttpClient();
        client.BaseAddress = new Uri(this.BaseUrl);

        // Send a GET request
        var response = client.GetAsync(this.Query).Result;

        // Parse the response body.
        if (response.IsSuccessStatusCode) {
            var data = response.Content.ReadAsAsync<Response>().Result;

            // Set the Pages and Emotes properties
            this.Pages = data.Pages;
            this.Emotes = data.Emotes;

            if (this.Emotes.Length == 0) {
                // Set status to false if the request failed
                this.StatusOk = false;
                this.Error = "No emotes found";
                return;
            }

            // Set status to true if the request succeeded
            this.StatusOk = true;
        } else {
            // Set status to false if the request failed
            this.StatusOk = false;
            this.Error = response.StatusCode.ToString();
        }
    }

    // GET specific emote
    public HttpResponseMessage GetEmote() {
            
            // Create a new HttpClient object.
            using var client = new HttpClient();
            client.BaseAddress = new Uri(this.BaseUrl);
    
            // Send a GET request
            var response = client.GetAsync(this.Query).Result;
    
            // Parse the response body.
            if (response.IsSuccessStatusCode) {
                this.StatusOk = true;
                return response;
            } else {
                // Set status to false if the request failed
                this.StatusOk = false;
                this.Error = response.StatusCode.ToString();
                return null;
            }
    }
}