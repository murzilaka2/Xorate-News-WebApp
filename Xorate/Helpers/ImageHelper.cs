namespace Xorate.Helpers
{
    public class ImageHelper
    {
        public static bool IsImagePath(string? path)
        {
            if (String.IsNullOrWhiteSpace(path))
            {
                return false;
            }
            if (!path.StartsWith("https://i.imgur.com/", StringComparison.OrdinalIgnoreCase) && 
                !path.StartsWith("i.imgur.com/", StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("http://i.imgur.com/", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".ico" };
            string extension = System.IO.Path.GetExtension(path).ToLower();
            return Array.Exists(validExtensions, ext => ext == extension);
        }
    }
}
