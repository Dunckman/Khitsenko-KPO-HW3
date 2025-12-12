using FileAnalysisService.Entities;

namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Результат работы детектора плагиата по одной сдаче
    /// </summary>
    public class PlagiarismDetectionResult
    {
        /// <summary>
        /// Совпадения с другими работами, найденные при анализе
        /// </summary>
        public IReadOnlyCollection<PlagiarismMatch> Matches { get; set; } = Array.Empty<PlagiarismMatch>();

        /// <summary>
        /// Максимальный процент схожести относительно всех совпавших работ
        /// </summary>
        public double MaxSimilarityPercentage { get; set; } = 0.0d;
    }
}