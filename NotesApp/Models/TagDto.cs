using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class TagDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва тегу обов'язкова")]
        [StringLength(50, ErrorMessage = "Назва не може перевищувати 50 символів")]
        public string Name { get; set; }

        public string Color { get; set; } = "#6c757d"; // Узгоджуємо з моделлю Tag
        public string UserId { get; set; }
        public bool IsCustom { get; set; }
    }
}
