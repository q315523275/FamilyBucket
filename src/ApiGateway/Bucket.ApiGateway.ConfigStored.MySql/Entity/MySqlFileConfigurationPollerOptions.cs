using Ocelot.Configuration.Repository;

namespace Bucket.ApiGateway.ConfigStored.MySql.Entity
{
    public class MySqlFileConfigurationPollerOptions : IFileConfigurationPollerOptions
    {
        public int Delay => 3000;
    }
}
