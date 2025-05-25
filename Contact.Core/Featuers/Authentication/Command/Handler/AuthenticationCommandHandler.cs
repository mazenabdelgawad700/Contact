using AutoMapper;
using Contact.Core.Featuers.Authentication.Command.Model;
using Contact.Domain.Entities;
using Contact.Service.Abstracts;
using Contact.Shared.Bases;
using MediatR;

namespace Contact.Core.Featuers.Authentication.Command.Handler
{
    public class AuthenticationCommandHandler : ReturnBase, IRequestHandler<RegisterUserCommand, ReturnBase<bool>>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IMapper _mapper;

        public AuthenticationCommandHandler(IAuthenticationService authenticationService, IMapper mapper)
        {
            _authenticationService = authenticationService;
            _mapper = mapper;
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

                return Success(true);
            }
            catch (Exception ex)
            {
                return Failed<bool>(ex.InnerException.Message);
            }
        }
    }
}
