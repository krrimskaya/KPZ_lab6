using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Services.Interfaces;

namespace NotesApp.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HistoryService(ApplicationDbContext context,
                              IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task RecordAsync(int noteId, string action, string title, string content)
        {
            var username = _httpContextAccessor
                               .HttpContext?
                               .User?
                               .Identity?
                               .Name
                           ?? "Unknown";

            var history = new NoteHistory
            {
                NoteId = noteId,
                ChangeType = action,
                Title = title,
                Content = content,
                ChangedAt = DateTime.UtcNow,
                ChangedBy = username
            };

            _context.NoteHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NoteHistory>> GetAllAsync(int? tagId = null)
        {
            var query = _context.NoteHistories
                .Include(h => h.Note)
                    .ThenInclude(n => n.NoteTags)
                        .ThenInclude(nt => nt.Tag)
                .AsQueryable();

            if (tagId.HasValue)
            {
                query = query.Where(h =>
                    h.Note.NoteTags.Any(nt => nt.TagId == tagId.Value));
            }

            return await query
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();
        }

        public async Task ClearAllAsync()
        {
            _context.NoteHistories.RemoveRange(_context.NoteHistories);
            await _context.SaveChangesAsync();
        }

        public async Task ClearByTagAsync(int tagId)
        {
            var toDelete = _context.NoteHistories
                .Include(h => h.Note)
                    .ThenInclude(n => n.NoteTags)
                .Where(h =>
                    h.Note.NoteTags.Any(nt => nt.TagId == tagId));

            _context.NoteHistories.RemoveRange(toDelete);
            await _context.SaveChangesAsync();
        }
    }
}
