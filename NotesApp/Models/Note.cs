using System;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReminderAt { get; set; }
    }
}
