using System.ComponentModel.DataAnnotations;

namespace Xorate.ViewModels
{
    public class ShortPostViewModel
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(70)]
        [Required]
        public string Name { get; set; }

        [MaxLength(1000)]
        [Required]
        public string Description { get; set; }

        [Required]
        public string? ImageLink { get; set; }

        [Range(minimum: 1, 5)]
        [Required]
        public float Ranking { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
