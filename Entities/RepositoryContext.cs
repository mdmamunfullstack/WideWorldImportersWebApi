using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;

namespace Entities
{
    public class RepositoryContext : WideWorldImportersDbContext
    {
        private readonly IOptions<AppConfigSettings> _appConfigSettings;

        public RepositoryContext(IOptions<AppConfigSettings> appConfigSettings) { _appConfigSettings = appConfigSettings; }

        public RepositoryContext(DbContextOptions<WideWorldImportersDbContext> options, IOptions<AppConfigSettings> appConfigSettings)
            : base(options)
        {
            _appConfigSettings = appConfigSettings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();

            if (_appConfigSettings.Value.LogSqlServer)
            {
                optionsBuilder.LogTo(logger.Debug);
                optionsBuilder.EnableSensitiveDataLogging();
            }
        }
    }
}