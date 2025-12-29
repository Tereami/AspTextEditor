using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspTextEditor.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("Имя")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DisplayName("Повторите пароль")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
