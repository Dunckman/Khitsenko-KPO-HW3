using System;

namespace AntiplagiatSystem. Shared. Exceptions
{
    /// <summary>
    /// Исключение, возникающее при ошибках в процессе анализа работы
    /// </summary>
    public class AnalysisException : Exception
    {
        /// <summary>
        /// Создаёт исключение с обобщённым описанием проблемы анализа
        /// </summary>
        public AnalysisException()
            : base("Во время анализа работы возникла непредвиденная ошибка")
        {
        }

        /// <summary>
        /// Создаёт исключение с заданным сообщением об ошибке
        /// </summary>
        /// <param name="message">Текст, описывающий суть проблемы</param>
        public AnalysisException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Создаёт исключение с сообщением и внутренней ошибкой
        /// </summary>
        /// <param name="message">Текст, описывающий суть проблемы</param>
        /// <param name="innerException">Исключение, ставшее причиной текущей ошибки</param>
        public AnalysisException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}