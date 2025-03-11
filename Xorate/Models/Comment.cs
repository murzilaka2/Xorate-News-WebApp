namespace Xorate.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTime CreatedDate { get; set; }

        public int PostId { get; set; }
        public Post? Post { get; set; }
    }
}
