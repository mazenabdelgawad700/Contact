using Contact.Domain.Entities;
using Contact.Shared.Bases;

namespace Contact.Service.Abstracts
{
    public interface IConfirmEmailService
    {
        Task<ReturnBase<bool>> SendConfirmationEmailAsync(User user);
        Task<ReturnBase<bool>> ConfirmEmailAsync(string userId, string token);
    }
}
