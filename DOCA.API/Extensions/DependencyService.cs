using System.Text;
using DOCA.API.Services.Implement;
using DOCA.API.Services.Interface;
using DOCA.Domain.Models;
using DOCA.Repository.Implement;
using DOCA.Repository.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;

namespace DOCA.API.Extensions;

public static class DependencyService
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork<DOCADbContext>, UnitOfWork<DOCADbContext>>();
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection service)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        service.AddDbContext<DOCADbContext>(options => options.UseSqlServer(CreateConnectionString(configuration)));
        service.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        service.AddScoped<DbContext, DOCADbContext>();
        
        return service;
    }
    private static string CreateConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("ConnectionStrings:MyConnectionString");
        return connectionString;
    }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserService, UserService>();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection service)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        service.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidIssuer = configuration["JWT:Issuer"],
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]!)),
                };
            });
        return service;
    }

    public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo() {Title = "DOCA System", Version = "v1"});
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
            options.MapType<TimeOnly>(() => new OpenApiSchema
            {
                Type = "string",
                Format = "time",
                Example = OpenApiAnyFactory.CreateFromJson("\"13:45:42.0000000\"")
            });
        });
        return services;
    }
    
}