using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

[Authorize]
[Route("Tag")]
[ApiController]
public class TagController : ControllerBase
{
    private readonly ITagRepository _tagRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public TagController(ITagRepository tagRepository, UserManager<IdentityUser> userManager)
    {
        _tagRepository = tagRepository;
        _userManager = userManager;
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        var userId = _userManager.GetUserId(User);
        var tags = await _tagRepository.GetUserTagsAsync(userId);
        return Ok(tags);
    }

    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] TagCreateRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = _userManager.GetUserId(User);
        var createdTag = await _tagRepository.CreateTagAsync(request.Name, request.Color, userId);
        return Ok(createdTag);
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);

        try
        {
            await _tagRepository.DeleteTagAsync(id, userId);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    public class TagCreateRequest
    {
        [Required(ErrorMessage = "Назва тегу обов'язкова")]
        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів")]
        public string Name { get; set; }

        public string Color { get; set; } = "#6c757d";
    }
}