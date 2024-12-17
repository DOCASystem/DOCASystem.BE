using System.Text;
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

    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        services.AddDbContext<DOCADbContext>(options => options.UseSqlServer(CreateConnectionString(configuration)));
        return services;
    }
    private static string CreateConnectionString(IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("ConnectionStrings:MyConnectionString");
        return connectionString;
    }
    
    private static string CreateClientId(IConfiguration configuration)
    {
        var clientId = configuration.GetValue<string>("Oauth:ClientId");
        return clientId;
    }
    private static string CreateClientSecret(IConfiguration configuration)
    {
        var clientSecret = configuration.GetValue<string>("Oauth:ClientSecret");
        return clientSecret;
    }
    
    private static IConfigurationSection CreateMailSettings(IConfiguration configuration)
    {
        var mailSettings = configuration.GetSection("MailSettings");
        return mailSettings;
    }

    // public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddScoped<IGradeLevelService, GradeLevelService>();
    //     services.AddScoped<IGradeService, GradeService>();
    //     services.AddScoped<ILessonService, LessonService>();
    //     services.AddScoped<IChapterService, ChapterService>();
    //     services.AddScoped<ISubjectService, SubjectService>();
    //     services.AddScoped<ICompetencyService, CompetencyService>();
    //     services.AddScoped<ICompetencyGroupService, CompetencyGroupService>();
    //     services.AddScoped<IContentAreaService, ContentAreaService>();
    //     services.AddScoped<ILearningTopicService, LearningTopicService>();
    //     services.AddScoped<ILessonItemService, LessonItemService>();
    //     services.AddScoped<ILessonLearningOutcomeService, LessonLearningOutcomeService>();
    //     services.AddScoped<IMasterSubjectService, MasterSubjectService>();
    //     services.AddScoped<IAccountService, AccountService>();
    //     services.AddScoped<ISchoolSupplierService, SchoolSupplierService>();
    //     services.AddScoped<ITeachingActivityService, TeachingActivityService>();
    //     services.AddScoped<IGoogleAuthenticationService, GoogleAuthenticationService>();
    //     services.AddScoped<ICurriculumService, CurriculumService>();
    //     services.AddScoped<ITextbookService, TextbookService>();
    //     services.AddScoped<ITextbookChapterService, TextbookChapterService>();
    //     services.AddScoped<GoogleDriveService>();
    //     services.AddScoped<ISyllabusService, SyllabusService>();
    //     services.AddScoped<IBlogService, BlogService>();
    //     services.AddScoped<ILabService, LabService>();
    //     services.AddScoped<ILabTutorialStepService, LabTutorialStepService>();
    //     return services;
    // }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidIssuer = "EducationCurriculum",
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("EducationCurriculumSecretKeyForJWTToken")),

            };
        })
        .AddCookie(options =>
            {
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
        .AddGoogle(options =>
        {
            options.ClientId = CreateClientId(configuration);
            options.ClientSecret = CreateClientSecret(configuration);
            options.SaveTokens = true;
            
        });
        
        return services;
    }

    public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo() {Title = "Education Curriculum", Version = "v1"});
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
    public static IServiceCollection AddMailService(this IServiceCollection services)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build();
        services.AddOptions();
        var mailSettings = CreateMailSettings(configuration);
        // services.Configure<MailSettings>(mailSettings);
        // services.AddTransient<ISendMailService, SendMailService>();
        return services;
    }
    
}