// File: Server/Services/LlmTutorService.cs
using System.Text.Json;
using Shared;

namespace Server.Services;

public class LlmTutorService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public LlmTutorService(HttpClient http, IConfiguration config)
    {
        _http = http;
        // Store this in appsettings.Development.json under "GeminiApiKey"
        _apiKey = config["GeminiApiKey"] ?? throw new ArgumentNullException("API Key missing");
    }

    public async Task<TutorResponseDto> EvaluateStepAsync(TutorRequestDto request)
    {
        string systemInstruction = $@"
            You are an expert Intelligent Tutoring System teaching Ordinary Differential Equations.
            Module: {request.ModuleId}. Problem: {request.OriginalProblem}.
            
            RULES:
            1. Evaluate the student's latest step (provided in LaTeX).
            2. If correct, confirm and ask for the next step.
            3. If incorrect, provide a guiding hint. DO NOT give the exact answer immediately.
            4. If the student has failed multiple times, provide a 'bottom out' hint explaining WHY the step is taken.
            5. Always output math using LaTeX formatting wrapped in $ or $$.";

        // Build the history for context
        var contents = new List<object>();
        foreach (var interaction in request.ConversationHistory)
        {
            contents.Add(new { role = "user", parts = new[] { new { text = interaction.StudentStep } } });
            contents.Add(new { role = "model", parts = new[] { new { text = interaction.AiFeedback } } });
        }
        // Add the current step
        contents.Add(new { role = "user", parts = new[] { new { text = request.StudentNewStep } } });

        var payload = new
        {
            system_instruction = new { parts = new[] { new { text = systemInstruction } } },
            contents = contents,
            generationConfig = new
            {
                response_mime_type = "application/json",
                temperature = 0.2
            }
        };

        var response = await _http.PostAsJsonAsync(
            $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-pro:generateContent?key={_apiKey}", 
            payload);

        response.EnsureSuccessStatusCode();
        var jsonResult = await response.Content.ReadAsStringAsync();
        
        // Parse the Gemini API response wrapper
        using var document = JsonDocument.Parse(jsonResult);
        var textResponse = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text").GetString();

        return JsonSerializer.Deserialize<TutorResponseDto>(textResponse ?? "{}", 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new TutorResponseDto();
    }
}