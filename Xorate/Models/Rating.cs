namespace Xorate.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
