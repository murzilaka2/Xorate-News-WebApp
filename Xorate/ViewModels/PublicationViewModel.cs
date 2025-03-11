using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Xorate.ViewModels
{
    public class PublicationViewModel
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Заголовок")]
        [Required(ErrorMessage = "Укажите заголовок записи.")]
        [MinLength(10, ErrorMessage = "Заголовок записи должен иметь длину от 10 символов.")]
        [MaxLength(70, ErrorMessage = "Заголовок записи должен иметь длину до 70 символов.")]
        public string? Title { get; set; }

        [Display(Name = "Описание")]
        [Required(ErrorMessage = "Укажите описание записи.")]
        [MinLength(500, ErrorMessage = "Минимальное количество символов записи - 500.")]
        public string? Description { get; set; }

        [Display(Name = "Ссылка на главное изображение (imgur.com). Формат изображения 3:1 (1536x512px).")]
        [Required(ErrorMessage = "Укажите ссылку на главное изображение записи.")]
        [Remote(action: "IsImagePath", controller: "Home", ErrorMessage = "Используйте прямую ссылку на изображение через сайт i.imgur.com.")]
        public string? Image { get; set; }

        [Display(Name = "Описание главного изображения (атрибут alt)")]
        [Required(ErrorMessage = "Добавьте описание для главное изображения.")]
        [MinLength(3, ErrorMessage = "Описание изображения должно иметь длину от 3 символов.")]
        [MaxLength(40, ErrorMessage = "Описание изображения должно иметь длину до 40 символов.")]
        public string? Alt { get; set; }

        [Display(Name = "Ключевое слово")]
        [Required(ErrorMessage = "Укажите ключевое слово записи.")]
        [MaxLength(70, ErrorMessage = "Ключевое слово должно иметь длину до 70 символов.")]
        public string? SeoKeywords { get; set; }

        [Display(Name = "Короткое описание. Не повторяйте содержимое основного описания. Обязательно используйте ключевое слово.")]
        [Required(ErrorMessage = "Укажите короткое описание записи.")]
        [MinLength(80, ErrorMessage = "Минимальная длинна описания - 80 символов.")]
        [MaxLength(160, ErrorMessage = "Максимальная длинна описания - 160 символов.")]
        public string? SeoDescription { get; set; }

        [Display(Name = "Капча")]
        [Required(ErrorMessage = "Укажите код капчи.")]
        [StringLength(4)]
        public string? CaptchaCode { get; set; }
        public string[]? Rating { get; set; }
        public string? Tags { get; set; }
        public int? Likes { get; set; }
        public int? Views { get; set; }
    }
}
