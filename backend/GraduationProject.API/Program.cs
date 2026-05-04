using GraduationProject.Application.Contracts.Services;
using GraduationProject.Application.Settings;
using GraduationProject.API.Extensions;
using GraduationProject.API.Middlewares;
using GraduationProject.API.Middlewares.Subscription;
using GraduationProject.Domain.Entities;
using GraduationProject.Infrastructure.BackgroundServices;
using GraduationProject.Infrastructure.Persistence;
using GraduationProject.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 0. Stripe SDK Initialization
// (sets the global API key used by `new PaymentIntentService()` etc.)
Stripe.StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// 1. Identity Setup
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 2. Settings & Services
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 3. Swagger & JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenJwtAuth();
builder.Services.AddCustomJwtAuth(builder.Configuration);

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(GraduationProject.Application.Mappings.MappingProfile));

// 5. API & Infrastructure Services
builder.Services.AddApiServices(builder.Configuration);

// 6. Background Services
builder.Services.AddHostedService<SubscriptionCleanupService>();

var app = builder.Build();

// 7. Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
        string[] roleNames = { "User", "Admin" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<int>(roleName));
            }
        }

        var userManager = services.GetRequiredService<UserManager<User>>();
        string adminEmail = "admin@graduation.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new User
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(adminUser, "P@ssw0rd123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding roles and admin user.");
    }
}

// 8. Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<LoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<SubscriptionMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
