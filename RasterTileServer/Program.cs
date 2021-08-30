
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RamMonitor;

namespace RasterTileServer
{


    public class Program
    {
        private static string s_ProgramDirectory;
        private static string s_CurrentDirectory;
        private static string s_BaseDirectory;
        private static string s_ExecutablePath;
        private static string s_ExecutableDirectory;
        private static string s_Executable;
        private static string s_ContentRootDirectory;

        // private static Logging.TrivialLogger s_logger;


        private static void DisplayError(System.Exception ex)
        {
            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);

            System.Exception thisError = ex;
            while (thisError != null)
            {
                System.Console.WriteLine(thisError.Message);
                System.Console.WriteLine(thisError.StackTrace);

                if (thisError.InnerException != null)
                {
                    System.Console.WriteLine(System.Environment.NewLine);
                    System.Console.WriteLine("Inner Exception:");
                } // End if (thisError.InnerException != null) 

                thisError = thisError.InnerException;
            } // Whend 

            System.Console.WriteLine(System.Environment.NewLine);
            System.Console.WriteLine(System.Environment.NewLine);
        } // End Sub DisplayError 


        static Program()
        {
            try
            {
                s_ProgramDirectory = System.IO.Path.GetDirectoryName(typeof(Program).Assembly.Location);
                s_CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
                s_BaseDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                s_ExecutablePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                s_ExecutableDirectory = System.IO.Path.GetDirectoryName(s_ExecutablePath);
                s_Executable = System.IO.Path.GetFileNameWithoutExtension(s_ExecutablePath);

                string logFilePath = null;
                string fileName = @"ServiceStartupLog.htm";

                System.Console.WriteLine(System.AppDomain.CurrentDomain.FriendlyName);
                


                if ("dotnet".Equals(s_Executable, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s_ContentRootDirectory = s_ProgramDirectory;
                    logFilePath = System.IO.Path.Combine(s_ProgramDirectory, fileName);
                }
                else if ("iisexpress".Equals(s_Executable, System.StringComparison.InvariantCultureIgnoreCase))
                {
                    s_ContentRootDirectory = s_ProgramDirectory;
                    logFilePath = System.IO.Path.Combine(s_ProgramDirectory, fileName);
                }
                else
                {
                    s_ContentRootDirectory = s_ExecutableDirectory;
                    logFilePath = System.IO.Path.Combine(s_ExecutableDirectory, fileName);
                }

                if (System.IO.File.Exists(logFilePath))
                    System.IO.File.Delete(logFilePath);

                // // s_logger = new Logging.HtmlLogger(@"D:\IDGLog.htm");
                //s_logger = new Logging.HtmlLogger(logFilePath);

                //s_logger.Log(Logging.LogLevel_t.Information, "Program Directory: {0}", s_ProgramDirectory);
                //s_logger.Log(Logging.LogLevel_t.Information, "Current Directory: {0}", s_CurrentDirectory);
                //s_logger.Log(Logging.LogLevel_t.Information, "Base Directory: {0}", s_BaseDirectory);
                //s_logger.Log(Logging.LogLevel_t.Information, "Logfile Directory: {0}", s_ContentRootDirectory);
                //s_logger.Log(Logging.LogLevel_t.Information, "Executable Path: {0}", s_ExecutablePath);
                //s_logger.Log(Logging.LogLevel_t.Information, "Executable Directory: {0}", s_ExecutableDirectory);
                //s_logger.Log(Logging.LogLevel_t.Information, "Executable: {0}", s_Executable);


            } // End Try 
            catch (System.Exception ex)
            {
                DisplayError(ex);
                System.Environment.Exit(ex.HResult);
            } // End Catch 

        }


        public static void Main(string[] args)
        {
            IHost host = CreateHostBuilder(args).Build();

            // https://docs.microsoft.com/en-us/dotnet/core/extensions/logging?tabs=command-line
            // this bs is only created after Starup has been called...
            ILogger<Program> logger = host.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Host created.");

            host.Run();
        }



        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                  .ConfigureLogging(
                        delegate(ILoggingBuilder logging )
                        {
                            // logging.ClearProviders();


                            // logging.AddFilter("System", LogLevel.Debug);
                            // logging.AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Information);
                            // logging.AddFilter<ConsoleLoggerProvider>("Microsoft", LogLevel.Trace));

                           // logging.AddFileLogger(
                           //    delegate (FileLoggerOptions options)
                           //    {
                           //        options.LogLevel = LogLevel.Information;
                           //        options.LogFilePath = System.IO.Path.Combine(s_ContentRootDirectory, @"RamMonitor.log.txt");
                           //        if (System.IO.File.Exists(options.LogFilePath))
                           //            System.IO.File.Delete(options.LogFilePath);
                           //    }
                           //);

                        }
                    )
                  .ConfigureServices(delegate (IServiceCollection coll) {
                      //coll.AddLogging();

                      //FileLoggerOptions opt = new FileLoggerOptions()
                      //{
                      //    LogFilePath = System.IO.Path.Combine(s_ProgramDirectory, "test.txt")
                      //};
                      
                      //ILoggerProvider prov = new FileLoggerProvider(Microsoft.Extensions.Options.Options.Create(opt));
                      //ILogger fl = prov.CreateLogger("");
                      //coll.AddSingleton(fl);
                  })
                   .ConfigureWebHostDefaults(webBuilder =>
                   {
                       webBuilder.UseStartup<Startup>();
                   });
        }


    }


}
