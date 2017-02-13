namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class AnalysisError
    {
        public AnalysisError(RepositoryAnalysisError error, string description)
        {
            Error = error;
            Description = description;
        }

        public RepositoryAnalysisError Error { get; set; }
        public string Description { get; set; }

        #region Overrides of Object

        public override string ToString()
        {
            return Error + " " + Description;
        }

        #endregion
    }
}