using School_Admission_System_Backend.Shared.Repository;

namespace School_Admission_System_Backend.Shared.Extensions.DependencyExtensions
{
    public static class RegisterRepository
    {
        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        }
    }
}
