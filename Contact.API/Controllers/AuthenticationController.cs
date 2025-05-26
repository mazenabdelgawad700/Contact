using Contact.API.Base;
using Contact.Core.Featuers.Authentication.Command.Model;
using Contact.Shared.Bases;
using Microsoft.AspNetCore.Mvc;

namespace Contact.API.Controllers
{
    [Route("api/authentication/[action]")]
    public class AuthenticationController : AppControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);
            return ReturnResult(result);
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
        {
            ReturnBase<bool> response = await Mediator.Send(command);
            return ReturnResult(response);
        }
    }
}
