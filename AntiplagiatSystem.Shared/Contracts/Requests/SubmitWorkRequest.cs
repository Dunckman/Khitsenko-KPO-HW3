namespace AntiplagiatSystem.Shared.Contracts.Requests
{
    /// <summary>
    /// Описывает данные, которые пользователь указывает при сдаче работы через API шлюза
    /// </summary>
    public class SubmitWorkRequest
    {
        /// <summary>
        /// Имя и фамилия студента
        /// </summary>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Обозначение учебной группы
        /// </summary>
        public string StudentGroup { get; set; } = string.Empty;

        /// <summary>
        /// Идентификатор задания, к которому относится работа
        /// </summary>
        public int WorkId { get; set; } = 0;

        /// <summary>
        /// Название задания в удобном для людей виде
        /// </summary>
        public string WorkTitle { get; set; } = string.Empty;
    }
}