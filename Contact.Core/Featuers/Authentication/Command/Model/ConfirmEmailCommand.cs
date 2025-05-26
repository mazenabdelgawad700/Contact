using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Model
{
    public class ConfirmEmailCommand : IRequest<ReturnBase<bool>>
    {
        public string UserId { get; set; }
        public string Token { get; set; }
    }
}
