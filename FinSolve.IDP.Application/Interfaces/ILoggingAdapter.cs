namespace FinSolve.IDP.Application.Interfaces
{
    public interface ILoggingAdapter
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message, Exception? ex = null);
    }
}
