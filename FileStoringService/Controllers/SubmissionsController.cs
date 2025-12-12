using AntiplagiatSystem. Shared.DTOs;
using FileStoringService.Repositories;
using Microsoft.AspNetCore. Mvc;

namespace FileStoringService.Controllers
{
    /// <summary>
    /// Позволяет получать информацию о сдачах работ
    /// </summary>
    [ApiController]
    [Route("api/submissions")]
    public class SubmissionsController : ControllerBase
    {
        private readonly ISubmissionRepository _submissionRepository;

        /// <summary>
        /// Создаёт экземпляр контроллера сдач работ
        /// </summary>
        /// <param name="submissionRepository">Репозиторий доступа к данным о сдачах</param>
        public SubmissionsController(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        /// <summary>
        /// Возвращает перечень всех сдач для конкретного задания
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список сдач по указанному заданию</returns>
        [HttpGet("by-work/{workId:int}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<WorkSubmissionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByWorkIdAsync(int workId, CancellationToken cancellationToken)
        {
            var submissions = await _submissionRepository.GetByWorkIdAsync(workId, cancellationToken);

            var result = submissions
                . Select(x => new WorkSubmissionDto
                {
                    SubmissionId = x.Id,
                    WorkId = x. WorkId,
                    WorkTitle = x.WorkTitle,
                    Student = new StudentInfoDto
                    {
                        StudentName = x.StudentName,
                        StudentGroup = x.StudentGroup
                    },
                    FileId = x.FileId,
                    SubmittedAt = x.SubmittedAt
                })
                .ToArray();

            return Ok(result);
        }

        /// <summary>
        /// Возвращает сдачи по заданию, которые были сделаны до указанной даты
        /// </summary>
        /// <param name="workId">Идентификатор задания</param>
        /// <param name="beforeDate">Граничная дата (не включая её)</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Список сдач, сделанных раньше указанного времени</returns>
        [HttpGet("by-work/{workId:int}/before/{beforeDate}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<WorkSubmissionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByWorkIdBeforeDateAsync(
            int workId,
            DateTime beforeDate,
            CancellationToken cancellationToken)
        {
            var submissions = await _submissionRepository. GetByWorkIdBeforeDateAsync(workId, beforeDate, cancellationToken);

            var result = submissions
                .Select(x => new WorkSubmissionDto
                {
                    SubmissionId = x.Id,
                    WorkId = x.WorkId,
                    WorkTitle = x. WorkTitle,
                    Student = new StudentInfoDto
                    {
                        StudentName = x. StudentName,
                        StudentGroup = x.StudentGroup
                    },
                    FileId = x.FileId,
                    SubmittedAt = x. SubmittedAt
                })
                .ToArray();

            return Ok(result);
        }

        /// <summary>
        /// Возвращает одну сдачу по её идентификатору
        /// </summary>
        /// <param name="submissionId">Идентификатор сдачи</param>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Описание найденной сдачи или 404, если её нет</returns>
        [HttpGet("{submissionId:guid}")]
        [ProducesResponseType(typeof(WorkSubmissionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(Guid submissionId, CancellationToken cancellationToken)
        {
            var submission = await _submissionRepository.GetByIdAsync(submissionId, cancellationToken);

            if (submission == null)
            {
                return NotFound();
            }

            var dto = new WorkSubmissionDto
            {
                SubmissionId = submission.Id,
                WorkId = submission.WorkId,
                WorkTitle = submission.WorkTitle,
                Student = new StudentInfoDto
                {
                    StudentName = submission.StudentName,
                    StudentGroup = submission.StudentGroup
                },
                FileId = submission.FileId,
                SubmittedAt = submission.SubmittedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Возвращает все сдачи в системе в порядке от новых к старым
        /// </summary>
        /// <param name="cancellationToken">Токен отмены</param>
        /// <returns>Все записи о сдачах</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<WorkSubmissionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var submissions = await _submissionRepository.GetAllAsync(cancellationToken);

            var result = submissions
                .Select(x => new WorkSubmissionDto
                {
                    SubmissionId = x.Id,
                    WorkId = x.WorkId,
                    WorkTitle = x.WorkTitle,
                    Student = new StudentInfoDto
                    {
                        StudentName = x.StudentName,
                        StudentGroup = x.StudentGroup
                    },
                    FileId = x.FileId,
                    SubmittedAt = x. SubmittedAt
                })
                .ToArray();

            return Ok(result);
        }
    }
}