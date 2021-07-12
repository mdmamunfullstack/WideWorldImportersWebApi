namespace Entities
{
    public interface IAppConfigSettings
    {
        bool LogSqlServer { get; set; }
        bool DeleteLog { get; set; }
    }

    public class AppConfigSettings
    {
        public bool LogSqlServer { get; set; }
        public bool DeleteLog { get; set; }
    }
}