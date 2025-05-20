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

    public async Task<AIResponse> GetGroqChatCompletionAsync(string userMessage)
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.groq.com/openai/v1/chat/completions"
        );

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        string systemPrompt = File.ReadAllText("utils/AiMentorPrompt.txt");

        var requestBody = new
        {
            model = "llama3-8b-8192", // or "llama3-70b-8192"
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userMessage },
            },
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
            var message = doc
                .RootElement.GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            Console.WriteLine($"Raw message string: {message}");


            var aiResponse = JsonSerializer.Deserialize<AIResponse>(message ?? "");
            return aiResponse
                ?? new AIResponse
                {
                    Explanation = "No explanation",
                    FollowUpQuestion = "Can you ask again?",
                };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to parse Groq response: " + ex.Message);
            throw new Exception("Invalid response format from Groq.");
        }
    }
}
