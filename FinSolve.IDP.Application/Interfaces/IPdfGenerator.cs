namespace FinSolve.IDP.Application.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] GeneratePdf(string title, string content);
    }
}
