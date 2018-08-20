using System.Collections.Generic;
using WCFGenerator.Common;
using WCFGenerator.Common.Infrastructure;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal interface ICodeClassGeneratorRepository : ICodeClassGenerator
    {
        string RepositoryName { get; }

        string FileName { get; }
    }
}