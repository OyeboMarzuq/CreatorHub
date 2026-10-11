using CreatorHub.Application.Interfaces.Repositories;
using CreatorHub.Application.Interfaces.Services;
using CreatorHub.Application.Services;
using CreatorHub.Infrastructure.Email;
using CreatorHub.Infrastructure.Repositories;
using CreatorHub.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CreatorHub.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICreatorRepository, CreatorRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IProfileUrlRepository, ProfileUrlRepository>();
        services.AddScoped<ICreatorSearchRepository, CreatorSearchRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<ICreatorService, CreatorService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ITagService, TagService>();
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<ISearchService, CreatorSearchService>();

        return services;
    }
}