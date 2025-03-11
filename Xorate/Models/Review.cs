using System.ComponentModel.DataAnnotations;

namespace Xorate.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(minimum: 1, 5)]
        public float Ranking { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsPublicated { get; set; } = false;
        public int Votes { get; set; }
        public int ShortPostId { get; set; }
        public ShortPost? ShortPost { get; set; }
    }
}
