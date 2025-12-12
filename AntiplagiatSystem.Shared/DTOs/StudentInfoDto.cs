namespace AntiplagiatSystem. Shared.DTOs
{
    /// <summary>
    /// Базовая информация о студенте, сдающем работу
    /// </summary>
    public class StudentInfoDto
    {
        /// <summary>
        /// Имя и фамилия студента
        /// </summary>
        public string StudentName { get; set; } = string.Empty;

        /// <summary>
        /// Обозначение учебной группы
        /// </summary>
        public string StudentGroup { get; set; } = string.Empty;
    }
}