using System.Collections.Generic;
using System.Threading.Tasks;
using NotesApp.Models;

namespace NotesApp.Services.Interfaces
{
    public interface IHistoryService
    {
        Task RecordAsync(int noteId, string action, string title, string content);
        Task<List<NoteHistory>> GetAllAsync(int? tagId = null);
        Task ClearAllAsync();
        Task ClearByTagAsync(int tagId);
    }
}
