using System.Text;
using System.Text.RegularExpressions;

namespace AntiplagiatSystem.Shared. Extensions
{
    /// <summary>
    /// Набор вспомогательных методов для работы со строками в контексте анализа текстов
    /// </summary>
    public static class StringExtensions
    {
        private static readonly Regex WhitespaceRegex = new(@"\s+", RegexOptions. Compiled);
        private static readonly Regex NonLetterRegex = new(@"[^a-zA-Zа-яА-Я0-9\s]", RegexOptions.Compiled);

        /// <summary>
        /// Приводит текст к упрощённой форме, пригодной для последующего сравнения
        /// </summary>
        /// <param name="source">Исходная строка, которую нужно нормализовать</param>
        /// <returns>Нормализованная строка без лишних символов и с единым форматом пробелов</returns>
        public static string NormalizeForAnalysis(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            var lowerCased = source.ToLowerInvariant();
            var lettersAndDigitsOnly = NonLetterRegex.Replace(lowerCased, " ");
            var compactWhitespace = WhitespaceRegex. Replace(lettersAndDigitsOnly, " ");

            return compactWhitespace.Trim();
        }

        /// <summary>
        /// Разбивает текст на последовательность слов, исключая пустые элементы
        /// </summary>
        /// <param name="source">Строка, из которой нужно извлечь слова</param>
        /// <returns>Набор слов в том порядке, в котором они встречаются в тексте</returns>
        public static IReadOnlyList<string> SplitToWords(this string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return [];
            }

            var normalized = source.NormalizeForAnalysis();

            var parts = normalized
                . Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .ToArray();

            return parts;
        }

        /// <summary>
        /// Строит текстовое представление n-грамм (шинглов) из последовательности слов
        /// </summary>
        /// <param name="words">Набор слов, по которому нужно построить шинглы</param>
        /// <param name="windowSize">Размер окна, определяющий длину шингла</param>
        /// <returns>Множество строк, каждая из которых описывает один шингл</returns>
        public static ISet<string> BuildShingles(this IReadOnlyList<string> words, int windowSize)
        {
            var shingles = new HashSet<string>(StringComparer. Ordinal);

            if (words.Count == 0 || windowSize <= 0)
            {
                return shingles;
            }

            // если слов меньше, чем размер окна, возвращаем весь текст как один шингл
            if (words.Count < windowSize)
            {
                var joinedShort = string.Join(' ', words);
                shingles.Add(joinedShort);
                return shingles;
            }

            var builder = new StringBuilder();

            // проходим скользящим окном по всем словам
            for (var index = 0; index <= words.Count - windowSize; index++)
            {
                builder.Clear();

                for (var offset = 0; offset < windowSize; offset++)
                {
                    if (offset > 0)
                    {
                        builder.Append(' ');
                    }

                    builder.Append(words[index + offset]);
                }

                var shingle = builder.ToString();
                shingles.Add(shingle);
            }

            return shingles;
        }
    }
}