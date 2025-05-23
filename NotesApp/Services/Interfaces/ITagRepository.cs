using NotesApp.Models;

namespace NotesApp.Services.Interfaces
{
    public interface ITagRepository
    {
        Task<List<TagDto>> GetUserTagsAsync(string userId);
        Task<TagDto> CreateTagAsync(string name, string color, string userId);
        Task DeleteTagAsync(int id, string userId);
        Task<bool> TagExistsAsync(int id);
        Task<bool> TagIsUsedAsync(int id);
    }
}
