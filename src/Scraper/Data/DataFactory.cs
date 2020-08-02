using Microsoft.Extensions.Configuration;
using Npgsql;

namespace coach_bags_selenium
{
    public class DataFactory
    {
        private readonly IConfiguration _config;

        public DataFactory(IConfiguration config)
        {
            _config = config;
        }

        public NpgsqlConnection GetConnection()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            var connectionString = _config.GetConnectionString("Postgres");
            return new NpgsqlConnection(connectionString);
        }

        public coach_bags_selenium.Data.DatabaseContext GetDatabaseContext()
        {
            var connectionString = _config.GetConnectionString("Postgres");
            return new coach_bags_selenium.Data.DatabaseContext(connectionString);
        }
    }
}
