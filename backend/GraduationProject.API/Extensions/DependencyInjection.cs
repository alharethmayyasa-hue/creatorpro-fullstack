using GraduationProject.Application.Contracts.Repositories;
using GraduationProject.Application.Contracts.Services;
using GraduationProject.Infrastructure.Extensions;
using GraduationProject.Infrastructure.Persistence.Repositories;
using GraduationProject.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace GraduationProject.API.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            services.AddEndpointsApiExplorer();
            services.AddCustomApiBehavior();
            services.AddInfrastructureServices(configuration);

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDatabase(configuration);
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IPlanService, PlanService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IDiscountService, DiscountService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<ICreditService, CreditService>();

            return services;
        }
    }
}

