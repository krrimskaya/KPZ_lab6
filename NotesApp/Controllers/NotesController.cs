using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var notes = await _context.Notes
                .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("~/Views/Home/Index.cshtml", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,ReminderAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;
                _context.Add(note);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var notes = await _context.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return View("~/Views/Home/Index.cshtml", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string content, DateTime? reminderAt)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("Title", "Заголовок обов'язковий");
                var notes = await _context.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
                return View("~/Views/Home/Index.cshtml", notes);
            }

            note.Title = title;
            note.Content = content;
            note.ReminderAt = reminderAt;

            try
            {
                _context.Update(note);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Notes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetNoteTags(int noteId)
        {
            var tags = await _context.NoteTags
                .Where(nt => nt.NoteId == noteId)
                .Include(nt => nt.Tag)
                .Select(nt => new
                {
                    id = nt.Tag.Id,
                    name = nt.Tag.Name,
                    color = nt.Tag.Color
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTagToNote(int noteId, int tagId)
        {
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == noteId);

            if (note == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(tagId);
            if (tag == null)
            {
                return NotFound();
            }

            if (!note.NoteTags.Any(nt => nt.TagId == tagId))
            {
                note.NoteTags.Add(new NoteTag { NoteId = noteId, TagId = tagId });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTagFromNote(int noteId, int tagId)
        {
            var noteTag = await _context.NoteTags
                .FirstOrDefaultAsync(nt => nt.NoteId == noteId && nt.TagId == tagId);

            if (noteTag != null)
            {
                _context.NoteTags.Remove(noteTag);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }
    }
}