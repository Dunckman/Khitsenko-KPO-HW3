namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Описывает способ оценки схожести двух текстов
    /// </summary>
    public interface ITextSimilarityCalculator
    {
        /// <summary>
        /// Возвращает оценку схожести двух текстов в процентах
        /// </summary>
        /// <param name="first">Первый текст для сравнения</param>
        /// <param name="second">Второй текст для сравнения</param>
        /// <returns>Число от 0 до 100, отражающее долю совпадений</returns>
        double CalculateSimilarity(string first, string second);
    }
}