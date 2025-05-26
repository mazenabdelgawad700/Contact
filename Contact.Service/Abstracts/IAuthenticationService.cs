using Contact.Domain.Entities;
using Contact.Shared.Bases;

namespace Contact.Service.Abstracts
{
    public interface IAuthenticationService
    {
        Task<ReturnBase<bool>> RegisterUserAsync(User user, string password);
        Task<ReturnBase<string>> LoginAsync(string email, string password, bool rememberMe);
        Task<ReturnBase<string>> RefreshTokenAsync(string accessToken);
    }
}
