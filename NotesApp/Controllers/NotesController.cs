using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Services;
using NotesApp.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly NoteService _noteService;
        private readonly IHistoryService _historyService;

        public NotesController(
            ApplicationDbContext context,
            NoteService noteService,
            IHistoryService historyService)
        {
            _context = context;
            _noteService = noteService;
            _historyService = historyService;
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
                await _historyService.RecordAsync(
                    note.Id,
                    "Created",
                    note.Title,
                    note.Content);

                return RedirectToAction(nameof(Index));
            }

            var all = await _noteService.GetAllNotesAsync();
            return View("~/Views/Home/Index.cshtml", all);
        }

        // GET: Notes/Edit/5
        [HttpGet]
        public async Task<IActionResult> GetNoteForEdit(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null) return NotFound();

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
            if (note == null) return NotFound();

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
            if (note == null) return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            await _historyService.RecordAsync(
                note.Id,
                "Deleted",
                note.Title,
                note.Content);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("Notes/AddTagToNote")]
        public async Task<IActionResult> AddTagToNote([FromBody] TagNoteDto dto)
        {
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == dto.NoteId);

            if (note == null)
                return NotFound("Note not found");

            if (!await _context.Tags.AnyAsync(t => t.Id == dto.TagId))
                return NotFound("Tag not found");

            if (note.NoteTags.Any(nt => nt.TagId == dto.TagId))
                return BadRequest("Tag already added");

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
                .FirstOrDefaultAsync(nt =>
                    nt.NoteId == noteId &&
                    nt.TagId == tagId);

            if (noteTag == null)
                return NotFound("Association not found");

            _context.NoteTags.Remove(noteTag);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
