namespace FinSolve.IDP.Domain.Exceptions
{
    public class UnsupportedFormatException : Exception
    {
        public UnsupportedFormatException(string format)
            : base($"Unsupported document format: {format}") { }
    }
}
