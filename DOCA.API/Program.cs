
using DOCA.API.Constants;
using DOCA.API.Extensions;
using DOCA.API.Middlewares;
using FluentValidation.AspNetCore;

var logger = NLog.LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"))
    .GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(CorConstant.PolicyName,
            policy => { policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod(); });
    });
    builder.Services.Configure<HostOptions>(hostOptions =>
    {
        hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    });
    builder.Services.AddDatabase();
    builder.Services.AddRedis(builder.Configuration);
    builder.Services.AddUnitOfWork();
    builder.Services.AddServices(builder.Configuration);
    builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    builder.Services.AddSettings();
    builder.Services.AddEndpointsApiExplorer(); 
    builder.Services.AddAuthorization();
    builder.Services.AddControllers();
    builder.Services.AddJwtAuthentication();
    builder.Services.AddConfigSwagger();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHttpContextAccessor();
    
    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(CorConstant.PolicyName);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseSwagger(options => { options.SerializeAsV2 = true; });
    app.UseHttpsRedirection();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    logger.Info("Starting application...");
    app.Run();
}
catch (Exception e)
{
    logger.Error("Stop program because of error: " + e.Message);
}
finally
{
    NLog.LogManager.Shutdown();
}