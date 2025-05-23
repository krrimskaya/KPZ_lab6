using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NoteService _noteService;

        public NotesController(ApplicationDbContext context, NoteService noteService)
        {
            _noteService = noteService;
            _context = context;
        }

        // GET: Notes
        public async Task<IActionResult> Index()
        {
            var notes = await _noteService.GetAllNotesAsync();
            return View("~/Views/Home/Index.cshtml", notes);
        }

        // POST: Notes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,ReminderAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                await _noteService.CreateNoteAsync(note);

                await RecordHistory(note.Id, "Created", note.Title, note.Content);
                return RedirectToAction(nameof(Index));
            }

            var notes = await _noteService.GetAllNotesAsync();
            return View("~/Views/Home/Index.cshtml", notes);
        }

        // GET: Notes/Edit/5
        [HttpGet]
        public async Task<IActionResult> GetNoteForEdit(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = note.Id,
                title = note.Title,
                content = note.Content,
                reminderAt = note.ReminderAt?.ToString("yyyy-MM-ddTHH:mm")
            });
        }

        // GET: Notes/Delete/5
        [HttpGet]
        public async Task<IActionResult> GetNoteForDelete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                id = note.Id,
                title = note.Title,
                content = note.Content
            });
        }

        // POST: Notes/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            await RecordHistory(note.Id, "Deleted", note.Title, note.Content);

            return RedirectToAction(nameof(Index));
        }

        private async Task RecordHistory(int noteId, string action, string title, string content)
        {
            var history = new NoteHistory
            {
                NoteId = noteId,
                ChangeType = action,
                Title = title,
                Content = content,
                ChangedAt = DateTime.Now,
                ChangedBy = User.Identity?.Name ?? "Unknown"
            };

            _context.NoteHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        [HttpPost]
        [Route("Notes/AddTagToNote")]
        public async Task<IActionResult> AddTagToNote([FromBody] TagNoteDto dto)
        {
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == dto.NoteId);

            if (note == null)
                return NotFound("Нотатку не знайдено");

            if (!await _context.Tags.AnyAsync(t => t.Id == dto.TagId))
                return NotFound("Тег не знайдено");

            // Уникати дублювання
            if (note.NoteTags.Any(nt => nt.TagId == dto.TagId))
                return BadRequest("Тег вже прив’язаний");

            note.NoteTags.Add(new NoteTag
            {
                NoteId = dto.NoteId,
                TagId = dto.TagId
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetNoteTags(int noteId)
        {
            var tags = await _context.NoteTags
                .Where(nt => nt.NoteId == noteId)
                .Include(nt => nt.Tag)
                .Select(nt => new {
                    id = nt.TagId,
                    name = nt.Tag.Name,
                    color = nt.Tag.Color
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Notes/RemoveTagFromNote")]
        public async Task<IActionResult> RemoveTagFromNote(int noteId, int tagId)
        {
            var noteTag = await _context.NoteTags
                .FirstOrDefaultAsync(nt => nt.NoteId == noteId && nt.TagId == tagId);

            if (noteTag == null)
                return NotFound("Не знайдено зв’язку між нотаткою та тегом");

            _context.NoteTags.Remove(noteTag);
            await _context.SaveChangesAsync();

            return Ok();
        }

    }
}