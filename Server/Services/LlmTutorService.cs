// File: Server/Services/LlmTutorService.cs
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Google.GenAI;
using Google.GenAI.Types;
using Shared;

namespace Server.Services;

public class LlmTutorService
{
    private readonly string _apiKey;

    public LlmTutorService(IConfiguration config)
    {
        _apiKey = config["GeminiApiKey"] ?? throw new ArgumentNullException("API Key missing");
    }

    public async Task<TutorResponseDto> EvaluateStepAsync(TutorRequestDto request)
    {
        // 1. Initialize the official client
        var client = new Client(apiKey: _apiKey);
        
        // 2. Fetch the textbook pedagogy from the RAG Problem Bank
        var moduleContext = ProblemBank.Modules[request.ModuleId];

        // 3. Build the highly constrained System Prompt
        // Note: C# requires double curly braces {{ and }} to escape them inside an interpolated string
        string systemInstructionText = $@"
            You are an expert Intelligent Tutoring System teaching Ordinary Differential Equations.
            The student is working on {moduleContext.Title}.
            The specific problem they are trying to solve is: {request.OriginalProblem}
            
            TEXTBOOK REFERENCE MATERIAL:
            {moduleContext.TextbookExcerpt}
             YOUR INSTRUCTIONS:
            1. You must act as a tutor, not an answer key. 
            2. Evaluate the student's latest step (provided in LaTeX) based STRICTLY on the pedagogical process described in the TEXTBOOK REFERENCE MATERIAL above.
            3. Do not just look for the final answer. Check if they applied the correct mathematical operation for the current stage of the process.
            4. If correct, confirm only what they did and give no hints to the next step.
            5. If a problem is in a certain form already, and the next logical step is the same as the current equation, don't make the student rewrite the equation
            6. If incorrect, provide a hint that points them back to the specific textbook rule they violated. DO NOT give them the exact correct mathematical step.
            7. Always output math using LaTeX formatting wrapped in $ or $$.
            8. Students are allowed to skip step, but if they do, if they are right tell them about all the steps they just did. If they are wrong, tell them to take it one step at a time
            9. IMPORTANT FORMATTING: Use plain text for your English explanations, and ONLY wrap the actual mathematical formulas and variables in $$ or $. NEVER wrap an entire English sentence in math delimiters.
               - BAD EXAMPLE: $$ The next step is to find the integrating factor \mu(x) = e^{{\int P(x)dx}} $$
               - GOOD EXAMPLE: The next step is to find the integrating factor $$ \mu(x) = e^{{\int P(x)dx}} $$
            10. IMPORTANT: You must return your response as a raw JSON object matching this EXACT structure:
           {{
                ""IsCorrectDirection"": true,
                ""LlmFeedback"": ""Your feedback here""
                ""IsProblemComplete"": false,
            }}
            NOTE: Set ""IsProblemComplete"" to true ONLY when the student has successfully provided the final, completely simplified explicit solution to the differential equation. Otherwise, it must be false.
            11. Don't say anything along the lines of "" refer to x rule in the textbook"" instead actually just say the rule";

        // 4. Configure the model to strictly return JSON
        var config = new GenerateContentConfig
        {
            SystemInstruction = new Content 
            { 
                Role = "system", 
                Parts = new List<Part> { new Part { Text = systemInstructionText } } 
            },
            ResponseMimeType = "application/json",
            Temperature = 0.2f
        };

        // 5. Build the Context Window (History + New Step)
        var contents = new List<Content>();
        
        foreach (var interaction in request.ConversationHistory)
        {
            contents.Add(new Content { Role = "user", Parts = new List<Part> { new Part { Text = interaction.StudentStep } } });
            contents.Add(new Content { Role = "model", Parts = new List<Part> { new Part { Text = interaction.AiFeedback } } });
        }
        
        contents.Add(new Content { Role = "user", Parts = new List<Part> { new Part { Text = request.StudentNewStep } } });

        // 6. The Fallback Array (Ordered from fastest/newest to most reliable/older)
        string[] fallbackModels = new[] 
        { 
            "gemini-3-flash-preview", 
            "gemini-2.5-flash", 
            "gemma-4-31b-it", 
            "gemini-2.5-pro" 
        };

        int maxAttempts = 4;
        int currentAttempt = 0;

        // 7. Execute the API Call with Cascade and JSON Retry Loop
        while (currentAttempt < maxAttempts)
        {
            // Pick a model based on the attempt number
            string currentModel = fallbackModels[currentAttempt % fallbackModels.Length];
            
            try
            {
                var response = await client.Models.GenerateContentAsync(
                    model: currentModel,
                    contents: contents,
                    config: config
                );

                string? textResponse = response.Text?.Trim();
                Console.WriteLine(textResponse);
                if (!string.IsNullOrEmpty(textResponse))
                {
                    // Failsafe: Clean up markdown ticks if the model was overly helpful
                    if (textResponse.StartsWith("```json"))
                    {
                        textResponse = textResponse.Substring(7, textResponse.Length - 10).Trim();
                    }
                    else if (textResponse.StartsWith("```"))
                    {
                        textResponse = textResponse.Substring(3, textResponse.Length - 6).Trim();
                    }
                    
                    // Attempt to parse the JSON. If it fails, it throws JsonException and triggers a retry.
                    var dto = JsonSerializer.Deserialize<TutorResponseDto>(textResponse, 
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (dto != null)
                    {
                        Console.WriteLine($"[Success] Successfully generated response using {currentModel}.");
                        return dto; // Success! Return immediately to the frontend.
                    }
                }
            }
            catch (JsonException ex)
            {
                
                Console.WriteLine($"[Warning] Model {currentModel} returned invalid JSON. Retrying... Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Warning] Model {currentModel} failed (API Error). Retrying... Error: {ex.Message}");
            }

            currentAttempt++;
        }

        // 8. Ultimate Failsafe: What if EVERY model is down or fails parsing?
        return new TutorResponseDto
        {
            IsCorrectDirection = false,
            LlmFeedback = "The tutor is having trouble formulating a response right now due to high server demand. Please try clicking submit again."
        };
    }
}