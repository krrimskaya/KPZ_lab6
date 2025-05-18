using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress(ErrorMessage = "Неправильний формат email")]
        public string Email { get; set; }
    }
}