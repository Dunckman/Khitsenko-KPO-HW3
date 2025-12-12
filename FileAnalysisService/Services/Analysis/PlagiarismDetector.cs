using AntiplagiatSystem.Shared.DTOs;

namespace FileAnalysisService.Services.Analysis
{
    /// <summary>
    /// Реализует базовый алгоритм поиска плагиата по шинглам среди работ по одному заданию
    /// </summary>
    public class PlagiarismDetector : IPlagiarismDetector
    {
        private readonly ITextSimilarityCalculator _similarityCalculator;

        /// <summary>
        /// Создаёт экземпляр детектора плагиата
        /// </summary>
        /// <param name="similarityCalculator">Сервис расчёта схожести текстов</param>
        public PlagiarismDetector(ITextSimilarityCalculator similarityCalculator)
        {
            _similarityCalculator = similarityCalculator;
        }

        /// <inheritdoc />
        public async Task<PlagiarismDetectionResult> DetectAsync(
            Guid currentSubmissionId,
            DateTime currentSubmittedAt,
            string currentText,
            IReadOnlyCollection<WorkSubmissionDto> otherSubmissions,
            CancellationToken cancellationToken)
        {
            var matches = new List<Entities.PlagiarismMatch>();

            var maxSimilarity = 0.0d;

            foreach (var submission in otherSubmissions)
            {
                if (submission.SubmissionId == currentSubmissionId)
                {
                    continue;   // пропускаем саму себя
                }

                if (submission.SubmittedAt >= currentSubmittedAt)
                {
                    continue;   // учитываем только более ранние сдачи
                }

                cancellationToken.ThrowIfCancellationRequested();

                // текст других работ сюда будет подставляться позже, на уровне AnalysisService
                // здесь мы только оцениваем схожесть, когда нам передали обе строки

                // временный плейсхолдер; реальную схему вызова делаем в AnalysisService
            }

            var result = new PlagiarismDetectionResult
            {
                Matches = matches,
                MaxSimilarityPercentage = maxSimilarity
            };

            return await Task.FromResult(result);
        }
    }
}