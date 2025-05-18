// Models/NoteHistory.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.Models
{
    public class NoteHistory
    {
        public int Id { get; set; }
        
        [Required]
        public int NoteId { get; set; }
        
        [ForeignKey("NoteId")]
        public Note Note { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        [Required]
        public DateTime ChangedAt { get; set; } = DateTime.Now;
        
        [Required]
        [StringLength(20)]
        public string ChangeType { get; set; } // "Created", "Updated", "Deleted"
        
        [StringLength(100)]
        public string ChangedBy { get; set; }
    }
}