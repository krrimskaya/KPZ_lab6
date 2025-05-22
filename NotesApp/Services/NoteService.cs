using NotesApp.Data;
using NotesApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Services
{
    public class NoteService
    {
        private readonly ApplicationDbContext _context;

        public NoteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Note>> GetAllNotesAsync()
        {
            var now = DateTime.Now;
            return await _context.Notes
                .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
                .Where(n => !n.ReminderAt.HasValue || n.ReminderAt > now)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Note> CreateNoteAsync(Note note)
        {
            note.CreatedAt = DateTime.Now;
            _context.Add(note);
            await _context.SaveChangesAsync();
            return note;
        }
    }
}
