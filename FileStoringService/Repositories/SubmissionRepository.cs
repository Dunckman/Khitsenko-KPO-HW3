using FileStoringService.Data;
using FileStoringService. Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStoringService.Repositories
{
    /// <summary>
    /// Репозиторий для работы со сдачами работ через Entity Framework Core
    /// </summary>
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly FileStoringDbContext _dbContext;

        /// <summary>
        /// Создаёт новый экземпляр репозитория сдач
        /// </summary>
        /// <param name="dbContext">Контекст базы данных сервиса хранения</param>
        public SubmissionRepository(FileStoringDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<WorkSubmission> AddAsync(WorkSubmission submission, CancellationToken cancellationToken)
        {
            await _dbContext. Submissions.AddAsync(submission, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return submission;
        }

        /// <inheritdoc />
        public async Task<WorkSubmission? > GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var submission = await _dbContext. Submissions
                .Include(x => x.File)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            return submission;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmission>> GetByWorkIdAsync(int workId, CancellationToken cancellationToken)
        {
            var submissions = await _dbContext. Submissions
                .Where(x => x.WorkId == workId)
                .Include(x => x.File)
                .OrderBy(x => x.SubmittedAt)
                .ToListAsync(cancellationToken);

            return submissions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmission>> GetByWorkIdBeforeDateAsync(
            int workId,
            DateTime beforeDate,
            CancellationToken cancellationToken)
        {
            var submissions = await _dbContext. Submissions
                .Where(x => x.WorkId == workId && x.SubmittedAt < beforeDate)
                .Include(x => x.File)
                .OrderBy(x => x. SubmittedAt)
                .ToListAsync(cancellationToken);

            return submissions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmission>> GetAllAsync(CancellationToken cancellationToken)
        {
            var submissions = await _dbContext.Submissions
                .Include(x => x.File)
                .OrderByDescending(x => x. SubmittedAt)
                .ToListAsync(cancellationToken);

            return submissions;
        }

        /// <inheritdoc />
        public async Task<IReadOnlyCollection<WorkSubmission>> GetByFileIdAsync(Guid fileId, CancellationToken cancellationToken)
        {
            var submissions = await _dbContext.Submissions
                . Where(x => x.FileId == fileId)
                .Include(x => x.File)
                .OrderBy(x => x.SubmittedAt)
                .ToListAsync(cancellationToken);

            return submissions;
        }
    }
}