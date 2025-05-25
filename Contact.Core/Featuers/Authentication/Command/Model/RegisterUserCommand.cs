using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Model
{
    public class RegisterUserCommand : IRequest<ReturnBase<bool>>
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
