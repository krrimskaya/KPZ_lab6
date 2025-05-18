using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "Email обов'язковий")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [StringLength(100, ErrorMessage = "Пароль повинен містити мінімум {2} і максимум {1} символів.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}