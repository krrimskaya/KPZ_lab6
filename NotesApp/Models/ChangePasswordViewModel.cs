using System.ComponentModel.DataAnnotations;

namespace NotesApp.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Поточний пароль обов'язковий")]
        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Новий пароль обов'язковий")]
        [StringLength(100, ErrorMessage = "Пароль повинен містити мінімум {2} і максимум {1} символів.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердіть новий пароль")]
        [Compare("NewPassword", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; }
    }
}