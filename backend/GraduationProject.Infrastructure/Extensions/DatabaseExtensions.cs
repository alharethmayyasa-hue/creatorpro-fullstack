using GraduationProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection; // لتعريف IServiceCollection
using Microsoft.Extensions.Configuration;     // لتعريف IConfiguration

namespace GraduationProject.Infrastructure.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    config.GetConnectionString("DefaultConnection")
                ));

            return services;
        }
    }
}
