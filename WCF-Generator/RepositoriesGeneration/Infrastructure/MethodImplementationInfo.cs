namespace VersionedRepositoryGeneration.Generator.Infrastructure
{
    internal class MethodImplementationInfo
    {
        public RepositoryMethod Method { get; set; }
        public string Key { get; set; }
        public string JoinName { get; set; }
        public bool RequiresImplementation { get; set; }
    }
}