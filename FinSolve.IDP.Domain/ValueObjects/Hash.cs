namespace FinSolve.IDP.Domain.ValueObjects
{
    public readonly record struct Hash(string Value)
    {
        public static Hash FromBytes(byte[] bytes) => new(Convert.ToHexString(bytes));
    }
}
