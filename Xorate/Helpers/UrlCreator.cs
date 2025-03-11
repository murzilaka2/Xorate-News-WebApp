using System.Text;
using System.Text.RegularExpressions;

namespace Xorate.Helpers
{
	public class UrlCreator
	{
		public static string CreateUrl(string postTitle)
		{
			postTitle = Regex.Replace(postTitle, @"[^\p{L}\p{N}\s]", "");
			postTitle = Regex.Replace(postTitle, @"\s+", " ").Trim();

			Dictionary<char, string> letters = new Dictionary<char, string>()
			{
				['а'] = "a", ['б'] = "b", ['в'] = "v",
				['г'] = "g", ['д'] = "d", ['е'] = "e",
				['ё'] = "yo", ['ж'] = "zh", ['з'] = "z",
				['и'] = "i", ['й'] = "j", ['к'] = "k",
				['л'] = "l", ['м'] = "m", ['н'] = "n",
				['о'] = "o", ['п'] = "p", ['р'] = "r",
				['с'] = "s", ['т'] = "t", ['у'] = "u",
				['ф'] = "f", ['х'] = "h", ['ц'] = "c",
				['ч'] = "ch", ['ш'] = "sh", ['щ'] = "sh",
				['ъ'] = "", ['ы'] = "y", ['ь'] = "",
				['ю'] = "yu", ['я'] = "ya",
				[' '] = "-", ['0'] = "0",
				['1'] = "1", ['2'] = "2", ['3'] = "3",
				['4'] = "4", ['5'] = "5", ['6'] = "6",
				['7'] = "7", ['8'] = "8", ['9'] = "9"
			};

			StringBuilder st = new StringBuilder();
			for (int i = 0;	i < postTitle.Length; i++)
			{
				char c = char.ToLowerInvariant(postTitle[i]);
				if (letters.ContainsKey(c))
				{
					st.Append(letters[c]);
				}
				else if (char.IsLetter(c))
				{
					st.Append(c);
				}
			}

			return st.ToString();
		}
	}
}
