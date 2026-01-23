using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Application.Interfaces
{
    public interface IPdfGenerator
    {
        byte[] Generate(ProcessingResult result);
    }
}
