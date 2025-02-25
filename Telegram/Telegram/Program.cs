void SendMessage(string message)
{
    var client = new HttpClient();
    var request = new HttpRequestMessage(HttpMethod.Post, "https://api.telegram.org/bot5949217810:AAFqqP1QyJAtrp4_jermdlLGBXiCzAqoJk8/sendMessage");
    var collection = new List<KeyValuePair<string, string>>();
    collection.Add(new("chat_id", "-1002370042141"));
    collection.Add(new("text", message));
    var content = new FormUrlEncodedContent(collection);
    request.Content = content;
    var response = client.SendAsync(request).Result;
    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
}



SendMessage("Hello, World!");