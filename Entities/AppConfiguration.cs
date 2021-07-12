using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WideWorldImportersWebApi
{
    public interface IAppConfiguration {
        bool LogSqlServer { get; set; }
        bool DeleteLog { get; set; }
    }

    public class AppConfiguration : IAppConfiguration
    {
        public bool LogSqlServer { get; set; }
        public bool DeleteLog { get; set; }
    }
}
