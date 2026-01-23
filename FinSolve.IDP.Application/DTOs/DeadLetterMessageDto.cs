namespace FinSolve.IDP.Application.DTOs
{
    public class DeadLetterMessageDto
    {
        public string Id { get; set; } = default!;
        public string OriginalQueue { get; set; } = default!;
        public string Body { get; set; } = default!;
        public string ErrorReason { get; set; } = default!;
        public string ErrorDescription { get; set; } = default!;
        public DateTime FailedAtUtc { get; set; }
    }

}