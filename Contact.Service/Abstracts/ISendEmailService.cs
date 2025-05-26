using Contact.Shared.Bases;

namespace Contact.Service.Abstracts
{
    public interface ISendEmailService
    {
        Task<ReturnBase<bool>> SendEmailAsync(string email, string message, string subject, string contentType = "text/plain");
    }
}
