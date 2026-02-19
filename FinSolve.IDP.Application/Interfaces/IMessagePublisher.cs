namespace FinSolve.IDP.Application.Interfaces
{
    public interface IMessagePublisher
    {
        Task PublishAsync(string topicName, object payload, string subject);
    }
}
