using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class TagController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TagController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = _userManager.GetUserId(User);
            var tags = await _context.Tags
                .Where(t => t.UserId == null || t.UserId == userId)
                .Select(t => new 
                {
                    id = t.Id,
                    name = t.Name,
                    color = t.Color,
                    userId = t.UserId,
                    isCustom = t.UserId != null
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name, string color)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Назва тегу обов'язкова");
            }

            var userId = _userManager.GetUserId(User);
            
            var tag = new Tag
            {
                Name = name.Trim(),
                Color = color ?? "#6c757d",
                UserId = userId
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new 
            {
                id = tag.Id,
                name = tag.Name,
                color = tag.Color,
                userId = tag.UserId,
                isCustom = tag.UserId != null
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (tag.UserId == null || tag.UserId != userId)
            {
                return Forbid("Ви не можете видалити цей тег");
            }

            var isUsed = await _context.NoteTags.AnyAsync(nt => nt.TagId == id);
            if (isUsed)
            {
                return BadRequest("Цей тег використовується в нотатках і не може бути видалений");
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}