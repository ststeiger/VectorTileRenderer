
namespace RamMonitor
{


    // https://github.com/serilog/serilog
    // https://github.com/serilog/serilog-aspnetcore/blob/dev/src/Serilog.AspNetCore/AspNetCore/RequestLoggingMiddleware.cs
    public class FileLoggerFactory
        : Microsoft.Extensions.Logging.ILoggerFactory
    {

        protected Microsoft.Extensions.Logging.ILoggerProvider m_provider;

        public FileLoggerFactory(Microsoft.Extensions.Logging.ILoggerProvider provider)
        {
            this.m_provider = provider;
        } // End Construtor 


        /*
        public FileLoggerFactory()
            : this(new FileLoggerProvider())
        { } // End Construtor 
        */

        void Microsoft.Extensions.Logging.ILoggerFactory.AddProvider(
            Microsoft.Extensions.Logging.ILoggerProvider provider)
        {
            this.m_provider = provider;

            throw new System.InvalidOperationException("Ignoring added logger provider");
        } // End Sub AddProvider 


        Microsoft.Extensions.Logging.ILogger Microsoft.Extensions.Logging.ILoggerFactory
            .CreateLogger(string categoryName)
        {
            return this.m_provider.CreateLogger(categoryName);
        } // End Function CreateLogger 


        void System.IDisposable.Dispose()
        {
            this.m_provider.Dispose();
        } // End Sub Dispose 


    } // End Class FileLoggerFactory 


}
