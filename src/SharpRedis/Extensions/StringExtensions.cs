using System;

namespace SharpRedis.Extensions
{
    unsafe public static class StringExtensions
    {
        public static bool Matches(string input, params string[] patterns)
        {
            var _input = input?.Trim();
            if (patterns is null || patterns.Length is 0 || string.IsNullOrEmpty(_input)) return false;
            fixed (char* pInput = _input)
                for (uint i = 0; i < patterns.Length; i++)
                    fixed (char* pPattern = patterns[i])
                        if (StringExtensions.MatchesRecursive(pInput, pPattern)) return true;
            return false;
        }

        private static bool MatchesRecursive(char* input, char* pattern)
        {
            if (*input == '\0' && *pattern == '\0') return true;
            if (*pattern == '\0') return false;

            if (*input == '\0')
            {
                while (*pattern == '*') pattern++;
                return *pattern == '\0';
            }

            if (*pattern == '*')
            {
                return StringExtensions.MatchesRecursive(input + 1, pattern) || StringExtensions.MatchesRecursive(input, pattern + 1);
            }
            else if (*pattern == '?')
            {
                return StringExtensions.MatchesRecursive(input + 1, pattern + 1);
            }
            else if (*pattern == '[')
            {
                char* closingBracket = pattern;
                while (*closingBracket != ']' && *closingBracket != '\0') closingBracket++;
                if (*closingBracket == '\0') throw new ArgumentException("Invalid pattern: missing ']'");

                char* temp = pattern + 1;
                while (temp < closingBracket)
                {
                    if (*temp == *input) return StringExtensions.MatchesRecursive(input + 1, closingBracket + 1);
                    temp++;
                }
                return false;
            }
            else
            {
                return *input == *pattern && StringExtensions.MatchesRecursive(input + 1, pattern + 1);
            }
        }
    }
}
