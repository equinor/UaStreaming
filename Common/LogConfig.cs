using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LogConfig
    {
        public static ILogger CreateLogger()
        {
            var loggerConfig = new LoggerConfiguration();

            if (Environment.UserInteractive)
            {
                loggerConfig
                    .MinimumLevel.Verbose()
                    .Enrich.WithExceptionDetails()
                    .Enrich.FromLogContext()
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Properties} {Level:u3}] {Message:lj}{NewLine}{Exception}");
            }

            else
            {
                loggerConfig
                    .MinimumLevel.Information()
                    .Enrich.WithExceptionDetails()
                    .Enrich.WithProperty("Application", "IP21Streamer")
                    .Enrich.WithMachineName()
                    .Enrich.WithCorrelationId()
                    .Enrich.FromLogContext()
                    .WriteTo.RollingFile(
                        pathFormat: Utils.GetLogPath(),
                        fileSizeLimitBytes: 10485760,
                        retainedFileCountLimit: 2,
                        formatter: new JsonFormatter(renderMessage: true))
                    .WriteTo.ApplicationInsightsTraces(Utils.GetAiInstrumentationKey());
            }

            Log.Logger = loggerConfig.CreateLogger();
            return Log.Logger;
        }

        public static void EndLogging() => Log.CloseAndFlush();
    }
}
