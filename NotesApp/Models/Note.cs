using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заголовок обов'язковий")]
        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? ReminderAt { get; set; }

        public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
    }
}