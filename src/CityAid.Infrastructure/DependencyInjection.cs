using CityAid.Application.Common.Interfaces;
using CityAid.Domain.Repositories;
using CityAid.Infrastructure.Persistence;
using CityAid.Infrastructure.Repositories;
using CityAid.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CityAid.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<CityAidDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Add repositories
        services.AddScoped<ICaseRepository, CaseRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Add services
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}