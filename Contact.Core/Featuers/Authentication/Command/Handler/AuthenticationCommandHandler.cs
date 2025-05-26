using AutoMapper;
using Contact.Core.Featuers.Authentication.Command.Model;
using Contact.Domain.Entities;
using Contact.Service.Abstracts;
using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Handler
{
    public class AuthenticationCommandHandler : ReturnBase, IRequestHandler<RegisterUserCommand, ReturnBase<bool>>,
        IRequestHandler<ConfirmEmailCommand, ReturnBase<bool>>,
        IRequestHandler<LoginCommand, ReturnBase<string>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfirmEmailService _confirmEmailService;
        private readonly IMapper _mapper;

        public AuthenticationCommandHandler(IAuthenticationService authenticationService, IMapper mapper, IConfirmEmailService confirmEmailService)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
            _confirmEmailService = confirmEmailService;
        }

        public async Task<ReturnBase<bool>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mappedResult = _mapper.Map<User>(request);
                if (mappedResult is null)
                    return Failed<bool>("Invalid user data");

                var registerationResult = await _authenticationService.RegisterUserAsync(mappedResult, request.Password);

                if (!registerationResult.Succeeded)
                    return Failed<bool>(registerationResult.Message);

                return Success(true, registerationResult.Message);
            }
            catch (Exception ex)
            {
                return Failed<bool>(ex.InnerException.Message);
            }
        }
        public async Task<ReturnBase<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var confirmEmailResult = await _confirmEmailService.ConfirmEmailAsync(request.UserId, request.Token);

                if (!confirmEmailResult.Succeeded)
                    return Failed<bool>(confirmEmailResult.Message);

                return Success(true, confirmEmailResult.Message);
            }
            catch (Exception ex)
            {
                return Failed<bool>(ex.InnerException.Message);
            }
        }
        public async Task<ReturnBase<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var loginResult = await _authenticationService.LoginAsync(request.Email, request.Password, request.RememberMe);

                if (!loginResult.Succeeded)
                    return Failed<string>(loginResult.Message);

                return Success(loginResult.Data, loginResult.Message);
            }
            catch (Exception ex)
            {
                return Failed<string>(ex.InnerException.Message);
            }
        }
    }
}
