using System.ComponentModel.DataAnnotations;

namespace Xorate.ViewModels
{
    public class CommentViewModel
    {
        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Укажите ваше имя.")]
        [MinLength(2, ErrorMessage = "Минимальное количество символов имени - 2.")]
        [MaxLength(50, ErrorMessage = "Максимальное количество символов имени - 50.")]
        public string Name { get; set; }
        
        [Display(Name = "Емейл")]
        [Required(ErrorMessage = "Укажите ваш емейл.")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Не корректный адрес емейл")]
        [EmailAddress(ErrorMessage = "Не корректный адрес емейл")]
        public string Email { get; set; }
        
        [Display(Name = "Комментарий")]
        [Required(ErrorMessage = "Укажите ваш комментарий.")]
        [MinLength(10, ErrorMessage = "Минимальное количество символов комментария - 10.")]
        [MaxLength(1000, ErrorMessage = "Максимальное количество символов комментария - 1000.")]
        public string Text { get; set; }
    }
}
