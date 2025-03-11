namespace Xorate.Models
{
	public class Post
	{
		public int Id { get; set; }
		public string Path { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public DateTime CreatedDate { get; set; }
		public string? Image { get; set; }
        public string? Alt { get; set; }

        public string? SeoKeywords { get; set; }
		public string? SeoDescription { get; set; }
		public string? Tags { get; set; }
		public int Likes { get; set; }
		public int Views { get; set; }
		public bool IsPublicated { get; set; } = false;

		public IEnumerable<Comment>? Comments{ get; set; }
		public IEnumerable<Rating>? Ratings { get; set; }
	}
}
