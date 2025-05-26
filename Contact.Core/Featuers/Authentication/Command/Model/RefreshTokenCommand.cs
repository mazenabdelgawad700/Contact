using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Model
{
    public class RefreshTokenCommand : IRequest<ReturnBase<string>>
    {
        public string AccessToken { get; set; }
    }
}
