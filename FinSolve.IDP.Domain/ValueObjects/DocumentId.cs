namespace FinSolve.IDP.Domain.ValueObjects
{
    public readonly record struct DocumentId(Guid Value)
    {
        public static DocumentId New() => new(Guid.NewGuid());
    }
}
