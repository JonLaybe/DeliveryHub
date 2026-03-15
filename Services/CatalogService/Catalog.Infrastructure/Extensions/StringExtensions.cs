namespace Catalog.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static List<string> GenerateSearchTokens(this string text)
        {
            const int minPrefixLength = 2;
            const int maxPrefixLength = 6;
            const int maxWords = 3;

            var tokens = new HashSet<string>();

            var words = text
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(maxWords);

            foreach (var word in words)
            {
                var length = Math.Min(word.Length, maxPrefixLength);

                for (int i = minPrefixLength; i <= length; i++)
                {
                    tokens.Add(word.Substring(0, i));
                }

                tokens.Add(word);
            }

            return tokens.ToList();
        }
    }
}
