namespace FinSolve.IDP.Domain.ValueObjects
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public List<string> Errors { get; }

        public ValidationResult(bool isValid, List<string> errors)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        public static ValidationResult Success() => new(true, []);
        public static ValidationResult Failure(params string[] errors) => new(false, errors.ToList());
    }

}
