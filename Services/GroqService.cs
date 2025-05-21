using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SOCRATIC_LEARNING_DOTNET.Entities;

namespace SOCRATIC_LEARNING_DOTNET.Services;

public class GroqService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GroqService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _apiKey = config["ApiKeys:Groq"];
    }

    public async Task<AIResponse> GetGroqChatCompletionAsync(IEnumerable<Message> messages)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        string systemPrompt = File.ReadAllText("utils/AiMentorPrompt.txt");

        var messageList = new List<object> { new { role = "system", content = systemPrompt } };

        foreach (var msg in messages)
        {
            messageList.Add(new { role = msg.role.ToLower(), content = msg.Content });
        }

        var requestBody = new
        {
            model = "llama3-8b-8192",
            messages = messageList,
            temperature = 0.5,
        };

        var json = JsonSerializer.Serialize(requestBody);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Groq API error details: " + errorDetails);
            throw new Exception($"Groq API error: {response.StatusCode} - {errorDetails}");
        }

        var content = await response.Content.ReadAsStringAsync();

        try
        {
            using var doc = JsonDocument.Parse(content);
            var messageContent = doc
                .RootElement.GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            Console.WriteLine($"Raw message string: {messageContent}");

            // Try parsing as JSON
            try
            {
                var responseDoc = JsonDocument.Parse(messageContent ?? "{}");
                var responseText = responseDoc.RootElement.GetProperty("Response").GetString();
                return new AIResponse { Response = responseText ?? "No response provided." };
            }
            catch
            {
                // If not JSON, fallback to raw text
                return new AIResponse { Response = messageContent ?? "No response provided." };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to parse Groq response: " + ex.Message);
            throw new Exception("Invalid response format from Groq.");
        }
    }
}
