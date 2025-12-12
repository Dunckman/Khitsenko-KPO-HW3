namespace AntiplagiatSystem.Shared.Enums
{
    /// <summary>
    /// Отражает итоговый вывод о наличии плагиата в проверяемой работе
    /// </summary>
    public enum PlagiarismVerdict
    {
        /// <summary>
        /// Существенных заимствований в работе не обнаружено
        /// </summary>
        NoPlagiarism = 0,

        /// <summary>
        /// Обнаружены совпадения, по которым есть повод присмотреться внимательнее
        /// </summary>
        SuspectedPlagiarism = 1,

        /// <summary>
        /// Обнаружено значительное количество совпадений, указывающее на плагиат
        /// </summary>
        ConfirmedPlagiarism = 2
    }
}