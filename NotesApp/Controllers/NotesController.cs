using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;

namespace NotesApp.Controllers
{
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Notes/Index - Показати всі нотатки
        public async Task<IActionResult> Index()
        {
            var notes = await _context.Notes.OrderByDescending(n => n.CreatedAt).ToListAsync();
            return View("~/Views/Home/Index.cshtml", notes);
        }

        // POST: Notes/Create - Створення нотатки через модальне вікно
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

        // POST: Notes/Edit - Редагування нотатки через модальне вікно
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string content, DateTime? reminderAt)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("Title", "Title is required");
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
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Notes/Delete - Видалення нотатки
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
                return NotFound();

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}