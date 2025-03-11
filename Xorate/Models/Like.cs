using System.ComponentModel.DataAnnotations;

namespace Xorate.Models
{
    public class Like
    {
        public int Id { get; set; }

        [MaxLength(160)]
        public string UniqueIdentifier{ get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
