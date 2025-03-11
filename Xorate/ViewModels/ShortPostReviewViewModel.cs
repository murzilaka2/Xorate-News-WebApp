using System.ComponentModel.DataAnnotations;

namespace Xorate.ViewModels
{
    public class ShortPostReviewViewModel
    {
        public int PostId { get; set; }

        [Required(ErrorMessage = "Укажите ваше имя.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Укажите ваш комментарий.")]
        public string? Description { get; set; }

        [Range(minimum: 1, 5)]
        public float Ranking { get; set; } = 0;
    }
}
