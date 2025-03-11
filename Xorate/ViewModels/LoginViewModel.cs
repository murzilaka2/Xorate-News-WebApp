using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Xorate.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name = "Емейл")]
        [Required(ErrorMessage = "Укажите емейл")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не корректный адрес емейл")]
        [EmailAddress(ErrorMessage = "Не корректный адрес емейл")]
        public string? Email { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Укажите пароль")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
