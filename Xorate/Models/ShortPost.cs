using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Xorate.ViewModels;

namespace Xorate.Models
{
    public class ShortPost
    {
        public int Id { get; set; }
        public string Path { get; set; }

        [MaxLength(70)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }
        public string? ImageLink { get; set; }

        [Range(minimum: 1, 5)]
        public float Ranking { get; set; } = 0;

        [NotMapped]
        public ShortPostReviewViewModel? ShortPostReviewViewModel { get; set; }

        public IEnumerable<Review>? Reviews { get; set; }
    }
}
