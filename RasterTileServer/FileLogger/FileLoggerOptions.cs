
namespace RamMonitor
{

    public class FileLoggerOptions
    {
        public FileLoggerOptions()
        { }


        public string LogFilePath { get; set; }
        
        public Microsoft.Extensions.Logging.LogLevel LogLevel { get; set; } =
            Microsoft.Extensions.Logging.LogLevel.Information;

    }

}
