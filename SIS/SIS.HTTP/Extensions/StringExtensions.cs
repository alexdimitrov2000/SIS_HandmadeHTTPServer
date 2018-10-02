namespace SIS.HTTP.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string word)
        {
            if (string.IsNullOrEmpty(word))
                return word;

            char[] wordChars = word.ToLower().ToCharArray();
            
            wordChars[0] = char.ToUpper(wordChars[0]);

            return new string(wordChars);
        }
    }
}
