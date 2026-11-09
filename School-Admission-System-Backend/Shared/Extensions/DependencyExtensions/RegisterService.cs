using School_Admission_System_Backend.Shared.Services;

namespace School_Admission_System_Backend.Shared.Extensions.DependencyExtensions
{
    public static class RegisterService
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
        }
    }
}
