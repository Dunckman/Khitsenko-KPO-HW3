using AntiplagiatSystem.Shared.Extensions;

namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Оценивает близость текстов с помощью шинглов и коэффициента Жаккара
    /// </summary>
    public class TextSimilarityCalculator :  ITextSimilarityCalculator
    {
        private const int DefaultWindowSize = 4;

        /// <inheritdoc />
        public double CalculateSimilarity(string first, string second)
        {
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(second))
            {
                return 0.0d;
            }

            var firstWords = first.SplitToWords();
            var secondWords = second.SplitToWords();

            var firstShingles = firstWords.BuildShingles(DefaultWindowSize);
            var secondShingles = secondWords.BuildShingles(DefaultWindowSize);

            if (firstShingles.Count == 0 || secondShingles.Count == 0)
            {
                return 0.0d;
            }

            // считаем количество совпадающих шинглов
            var intersection = firstShingles.Intersect(secondShingles).Count();

            // считаем количество уникальных шинглов в обоих текстах
            var union = firstShingles.Union(secondShingles).Count();

            if (union == 0)
            {
                return 0.0d;
            }

            // применяем коэффициент Жаккара
            var jaccard = (double)intersection / union;

            var percentage = jaccard * 100.0d;

            return percentage;
        }
    }
}