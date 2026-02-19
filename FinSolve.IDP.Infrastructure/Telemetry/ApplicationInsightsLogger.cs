using FinSolve.IDP.Application.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace FinSolve.IDP.Infrastructure.Telemetry
{
    public class ApplicationInsightsLogger : ILoggingAdapter
    {
        private readonly TelemetryClient _telemetry;

        public ApplicationInsightsLogger(TelemetryClient telemetry)
        {
            _telemetry = telemetry;
        }

        public void LogInformation(string message)
        {
            _telemetry.TrackTrace(message, SeverityLevel.Information);
        }

        public void LogWarning(string message)
        {
            _telemetry.TrackTrace(message, SeverityLevel.Warning);
        }

        public void LogError(string message, Exception? ex = null)
        {
            var trace = new TraceTelemetry(message, SeverityLevel.Error);
            if (ex != null)
            {
                trace.Properties["Exception"] = ex.ToString();
            }

            _telemetry.TrackTrace(trace);
        }
    }
}
