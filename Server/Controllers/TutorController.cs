// File: Server/Controllers/TutorController.cs
using Microsoft.AspNetCore.Mvc;
using Shared;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorController : ControllerBase
{
    private readonly LlmTutorService _tutorService;

    public TutorController(LlmTutorService tutorService)
    {
        _tutorService = tutorService;
    }

    [HttpPost("evaluate")]
    public async Task<ActionResult<TutorResponseDto>> Evaluate([FromBody] TutorRequestDto request)
    {
        var response = await _tutorService.EvaluateStepAsync(request);
        return Ok(response);
    }
}