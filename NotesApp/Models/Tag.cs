using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesApp.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public string Color { get; set; } = "#6c757d"; // Сірий за замовчуванням

        public string? UserId { get; set; } // Null для стандартних тегів

        // Відношення багато-до-багатьох для нотаток
        public ICollection<NoteTag> NoteTags { get; set; } = new List<NoteTag>();
    }
}