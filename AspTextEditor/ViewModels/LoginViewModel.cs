using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AspTextEditor.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("Имя")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DisplayName("Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        [HiddenInput(DisplayValue = false)]
        public string? ReturnUrl { get; set; }
    }
}
