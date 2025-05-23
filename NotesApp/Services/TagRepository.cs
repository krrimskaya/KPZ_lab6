using NotesApp.Data;
using NotesApp.Models;
using NotesApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace NotesApp.Services
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TagDto>> GetUserTagsAsync(string userId)
        {
            return await _context.Tags
                .Where(t => t.UserId == null || t.UserId == userId)
                .Select(t => new TagDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Color = t.Color,
                    UserId = t.UserId,
                    IsCustom = t.UserId != null
                })
                .ToListAsync();
        }

        public async Task<TagDto> CreateTagAsync(string name, string color, string userId)
        {
            var tag = new Tag
            {
                Name = name,
                Color = color ?? "#6c757d",
                UserId = userId
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                Color = tag.Color,
                UserId = tag.UserId,
                IsCustom = tag.UserId != null
            };
        }

        public async Task DeleteTagAsync(int id, string userId)
        {
            var tag = await _context.Tags
                .Include(t => t.NoteTags)
                .FirstOrDefaultAsync(t => t.Id == id);

            switch (tag)
            {
                case null:
                    throw new ArgumentException("Тег не знайдено");
                case { UserId: var uid } when uid != userId:
                    throw new UnauthorizedAccessException("Немає прав для видалення цього тегу");
                case { NoteTags.Count: > 0 }:
                    throw new InvalidOperationException("Неможливо видалити тег, що використовується в нотатках");
            }

            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }


        public async Task<bool> TagExistsAsync(int id)
        {
            return await _context.Tags.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TagIsUsedAsync(int id)
        {
            return await _context.NoteTags.AnyAsync(nt => nt.TagId == id);
        }
    }
}
