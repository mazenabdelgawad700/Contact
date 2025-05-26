using Contact.Service.Abstracts;
using Contact.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Contact.Service
{
    public static class ModuleServiceDependencies
    {
        public static IServiceCollection AddServiceDependencies(this IServiceCollection services)
        {
            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<ISendEmailService, SendEmailService>();
            services.AddTransient<IConfirmEmailService, ConfirmEmailService>();
            return services;
        }
    }
}
