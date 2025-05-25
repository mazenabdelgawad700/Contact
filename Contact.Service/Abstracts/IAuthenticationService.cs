using Contact.Domain.Entities;
using Contact.Shared.Bases;

namespace Contact.Service.Abstracts
{
    public interface IAuthenticationService
    {
        Task<ReturnBase<bool>> RegisterUserAsync(User user, string password);
    }
}
