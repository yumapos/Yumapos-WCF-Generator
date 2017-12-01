using Newtonsoft.Json.Linq;

namespace TestSerializationGeneration.Infrastructure
{

    public interface IMigrationBackup
    {
        JObject Migrate(JObject oldValue);
        int MigrationVersion { get; }
    }
}

