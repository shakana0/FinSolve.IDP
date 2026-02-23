using FinSolve.IDP.Domain.Entities;

namespace FinSolve.IDP.Application.Interfaces
{
    public interface IReportGenerator
    {
        byte[] Generate(ProcessingResult result);
    }
}
