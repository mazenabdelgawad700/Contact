using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Model
{
    public class LoginCommand : IRequest<ReturnBase<string>>
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
