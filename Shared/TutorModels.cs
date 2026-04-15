// File: Shared/TutorModels.cs
namespace Shared;

public class TutorRequestDto
{
    public string ModuleId { get; set; } = string.Empty;
    public string OriginalProblem { get; set; } = string.Empty;
    public string StudentNewStep { get; set; } = string.Empty;
    public List<StepInteraction> ConversationHistory { get; set; } = new();
}

public class TutorResponseDto
{
    public bool IsCorrectDirection { get; set; }
    public string LlmFeedback { get; set; } = string.Empty;
}

public class StepInteraction
{
    public string StudentStep { get; set; } = string.Empty;
    public string AiFeedback { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}