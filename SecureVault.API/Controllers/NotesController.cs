using Microsoft.AspNetCore.Mvc;
using SecureVault.Application.DTOs;
using SecureVault.Application.Interfaces;

namespace SecureVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotesController : ControllerBase
{
    private readonly INotesService _notesService;
    private readonly ILogger<NotesController> _logger;

    public NotesController(
        INotesService notesService,
        ILogger<NotesController> logger)
    {
        _notesService = notesService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(NoteResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NoteResponseDto>> CreateNote(
        [FromBody] CreateNoteDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _notesService.CreateNoteAsync(dto, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error });

        return CreatedAtAction(
            nameof(GetNote),
            new { id = result.Data!.Id },
            result.Data);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NoteResponseDto>> GetNote(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _notesService.GetNoteAsync(id, cancellationToken);

        if (!result.IsSuccess)
            return NotFound(new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NoteResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<NoteResponseDto>>> GetUserNotes(
        string userId,
        CancellationToken cancellationToken)
    {
        var result = await _notesService.GetUserNotesAsync(userId, cancellationToken);

        if (!result.IsSuccess)
            return StatusCode(500, new { error = result.Error });

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NoteResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NoteResponseDto>> UpdateNote(
        int id,
        [FromBody] UpdateNoteDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _notesService.UpdateNoteAsync(id, dto, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("not found"))
                return NotFound(new { error = result.Error });
            
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteNote(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _notesService.DeleteNoteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("not found"))
                return NotFound(new { error = result.Error });
            
            return StatusCode(500, new { error = result.Error });
        }

        return NoContent();
    }
}