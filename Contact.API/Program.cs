using Contact.Core;
using Contact.Infrastructure;
using Contact.Infrastructure.Context;
using Contact.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Contact.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            builder.Services.AddDbContext<AppDbContext>(
            options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("constr"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
            );

            builder.Services
                .AddServiceDependencies()
                .AddInfrastructureDependencies()
                .AddCoreDependencies()
                .AddServiceRegisteration(builder.Configuration);



            string CORS = "_cors";
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: CORS, policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                    policy.AllowAnyOrigin();
                });
            });


            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.UseCors(CORS);

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();


            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (!roleManager.Roles.Any())
                {
                    string[] roleNames = ["Admin", "User"];
                    foreach (var roleName in roleNames)
                    {
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
                }
                scope.Dispose();
            }



            app.Run();
        }
    }
}
