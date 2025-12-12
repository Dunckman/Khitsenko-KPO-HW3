using AntiplagiatSystem.Shared.Contracts. Requests;
using AntiplagiatSystem.Shared.Contracts. Responses;
using AntiplagiatSystem.Shared.DTOs;
using AntiplagiatSystem.Shared. Enums;
using FileAnalysisService. Entities;
using FileAnalysisService. Repositories;
using FileAnalysisService.Services. Clients;
using FileAnalysisService.Services.WordCloud;

namespace FileAnalysisService. Services. Analysis
{
    /// <summary>
    /// Реализует полный цикл анализа работ на плагиат
    /// </summary>
    public class AnalysisService : IAnalysisService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IFileStoringClient _fileStoringClient;
        private readonly ITextSimilarityCalculator _similarityCalculator;
        private readonly IWordCloudService _wordCloudService;

        /// <summary>
        /// Создаёт экземпляр сервиса анализа
        /// </summary>
        /// <param name="reportRepository">Репозиторий отчётов</param>
        /// <param name="fileStoringClient">Клиент для доступа к файлам и сдачам</param>
        /// <param name="similarityCalculator">Сервис расчёта схожести текстов</param>
        /// <param name="wordCloudService">Сервис построения облака слов</param>
        public AnalysisService(
            IReportRepository reportRepository,
            IFileStoringClient fileStoringClient,
            ITextSimilarityCalculator similarityCalculator,
            IWordCloudService wordCloudService)
        {
            _reportRepository = reportRepository;
            _fileStoringClient = fileStoringClient;
            _similarityCalculator = similarityCalculator;
            _wordCloudService = wordCloudService;
        }

        /// <inheritdoc />
        public async Task<AnalysisReportResponse> RunAnalysisAsync(
            AnalyzeFileRequest request,
            CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;

            // создаём заготовку отчёта со статусом "в процессе"
            var report = new AnalysisReport
            {
                Id = Guid.NewGuid(),
                SubmissionId = request.SubmissionId,
                FileId = request.FileId,
                WorkId = request.WorkId,
                Status = AnalysisStatus.InProgress,
                Verdict = PlagiarismVerdict.NoPlagiarism,
                SimilarityPercentage = 0.0d,
                WordCloudUrl = string.Empty,
                CreatedAt = now,
                CompletedAt = null,
                ErrorMessage = null,
                Matches = new List<PlagiarismMatch>()
            };

            await _reportRepository.AddAsync(report, cancellationToken);

            try
            {
                // получаем текст анализируемой работы
                var currentText = await _fileStoringClient.GetFileTextAsync(request.FileId, cancellationToken);

                // загружаем все сдачи по заданию, чтобы найти текущую
                var allSubmissions = await _fileStoringClient.GetSubmissionsByWorkIdAsync(
                    request.WorkId,
                    cancellationToken);

                var currentSubmission = allSubmissions. FirstOrDefault(x => x. SubmissionId == request.SubmissionId);

                if (currentSubmission == null)
                {
                    throw new InvalidOperationException("Сдача, указанная для анализа, не найдена в сервисе хранения");
                }

                // загружаем только те сдачи, которые были сделаны раньше текущей
                var earlierSubmissions = await _fileStoringClient.GetSubmissionsByWorkIdBeforeDateAsync(
                    request. WorkId,
                    currentSubmission. SubmittedAt,
                    cancellationToken);

                var matches = new List<PlagiarismMatch>();
                var maxSimilarity = 0.0d;

                // сравниваем текущую работу с каждой из ранних
                foreach (var submission in earlierSubmissions)
                {
                    if (submission. SubmissionId == request.SubmissionId)
                    {
                        continue;
                    }

                    cancellationToken.ThrowIfCancellationRequested();

                    var otherText = await _fileStoringClient.GetFileTextAsync(
                        submission.FileId,
                        cancellationToken);

                    var similarity = _similarityCalculator. CalculateSimilarity(currentText, otherText);

                    if (similarity <= 0.0d)
                    {
                        continue;
                    }

                    // обновляем максимальную схожесть
                    if (similarity > maxSimilarity)
                    {
                        maxSimilarity = similarity;
                    }

                    // фиксируем найденное совпадение
                    var match = new PlagiarismMatch
                    {
                        Id = Guid.NewGuid(),
                        ReportId = report.Id,
                        MatchedSubmissionId = submission.SubmissionId,
                        MatchedStudentName = submission.Student.StudentName,
                        SimilarityPercentage = similarity,
                        MatchedSubmissionDate = submission.SubmittedAt
                    };

                    matches. Add(match);
                }

                report.Matches = matches;
                report.SimilarityPercentage = maxSimilarity;
                report. Verdict = ResolveVerdict(maxSimilarity);

                // строим облако слов для текущей работы
                var wordCloudUrl = await _wordCloudService.BuildWordCloudUrlAsync(currentText, cancellationToken);

                report.WordCloudUrl = wordCloudUrl;
                report.Status = AnalysisStatus. Completed;
                report.CompletedAt = DateTime.UtcNow;

                await _reportRepository.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                // если произошла ошибка, фиксируем её в отчёте
                report.Status = AnalysisStatus.Failed;
                report.ErrorMessage = ex.Message;
                report. CompletedAt = DateTime. UtcNow;

                await _reportRepository.SaveChangesAsync(cancellationToken);

                throw;
            }

            var dto = MapToDto(report);

            var response = new AnalysisReportResponse
            {
                Report = dto
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<AnalysisReportResponse? > GetReportByIdAsync(Guid reportId, CancellationToken cancellationToken)
        {
            var report = await _reportRepository.GetByIdAsync(reportId, cancellationToken);

            if (report == null)
            {
                return null;
            }

            var dto = MapToDto(report);

            var response = new AnalysisReportResponse
            {
                Report = dto
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<AnalysisReportResponse?> GetReportBySubmissionAsync(
            Guid submissionId,
            CancellationToken cancellationToken)
        {
            var report = await _reportRepository. GetBySubmissionIdAsync(submissionId, cancellationToken);

            if (report == null)
            {
                return null;
            }

            var dto = MapToDto(report);

            var response = new AnalysisReportResponse
            {
                Report = dto
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<WorkReportsResponse> GetReportsByWorkIdAsync(int workId, CancellationToken cancellationToken)
        {
            var reports = await _reportRepository.GetByWorkIdAsync(workId, cancellationToken);

            var reportDtos = reports
                .Select(MapToDto)
                .ToArray();

            var response = new WorkReportsResponse
            {
                WorkId = workId,
                WorkTitle = string.Empty,
                Reports = reportDtos
            };

            return response;
        }

        /// <summary>
        /// Определяет вердикт о плагиате на основе максимального процента схожести
        /// </summary>
        /// <param name="maxSimilarity">Максимальный процент схожести с другими работами</param>
        /// <returns>Итоговый вердикт</returns>
        private static PlagiarismVerdict ResolveVerdict(double maxSimilarity)
        {
            if (maxSimilarity < 20.0d)
            {
                return PlagiarismVerdict.NoPlagiarism;
            }

            if (maxSimilarity < 50.0d)
            {
                return PlagiarismVerdict.SuspectedPlagiarism;
            }

            return PlagiarismVerdict. ConfirmedPlagiarism;
        }

        /// <summary>
        /// Преобразует сущность отчёта в DTO для передачи клиенту
        /// </summary>
        /// <param name="report">Сущность отчёта из базы данных</param>
        /// <returns>DTO отчёта</returns>
        private static ReportDto MapToDto(AnalysisReport report)
        {
            var dto = new ReportDto
            {
                ReportId = report.Id,
                SubmissionId = report. SubmissionId,
                FileId = report.FileId,
                WorkId = report.WorkId,
                Status = report.Status,
                Verdict = report. Verdict,
                SimilarityPercentage = report.SimilarityPercentage,
                WordCloudUrl = report. WordCloudUrl,
                CreatedAt = report.CreatedAt,
                CompletedAt = report.CompletedAt,
                ErrorMessage = report.ErrorMessage
            };

            return dto;
        }
    }
}