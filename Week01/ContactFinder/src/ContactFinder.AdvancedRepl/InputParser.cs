using System;
using System.Collections.Generic;
using System.Text;

namespace ContactFinder.AdvancedRepl
{
    public static class InputParser
    {
        public static (string command, string[] args) Parse(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return (string.Empty, Array.Empty<string>());

            var tokens = Tokenize(input);

            if (tokens.Count == 0)
                return (string.Empty, Array.Empty<string>());

            var command = tokens[0].ToLowerInvariant();
            var args = tokens.GetRange(1, tokens.Count - 1).ToArray();

            return (command, args);
        }

        private static List<string> Tokenize(string input)
        {
            var result = new List<string>();
            var sb = new StringBuilder();
            bool inQuotes = false;

            foreach (var c in input)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0)
                    {
                        result.Add(sb.ToString());
                        sb.Clear();
                    }
                    continue;
                }

                sb.Append(c);
            }

            if (sb.Length > 0)
                result.Add(sb.ToString());

            return result;
        }
    }
}
