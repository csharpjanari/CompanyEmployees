using Api.Extensions;
using AspNetCoreRateLimit;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Presentation.ActionFilters;
using Service;
using Shared.DataTransferObjects;

var builder = WebApplication.CreateBuilder(args);
{
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

    builder.Services.ConfigureCors();
    builder.Services.ConfigureLoggerService();
    builder.Services.ConfigureRepositoryManager();
    builder.Services.ConfigureServiceManager();
    builder.Services.ConfigureSqlContext(builder.Configuration);
    builder.Services.ConfigureVersioning();
    builder.Services.ConfigureResponseCaching();
    builder.Services.ConfigureHttpCacheHeaders();
    builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddMemoryCache();
    builder.Services.ConfigureRateLimitingOptions();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAuthentication();
    builder.Services.ConfigureIdentity();

    builder.Services.AddScoped<ValidationFilterAttribute>();
    builder.Services.AddScoped<IDataShaper<EmployeeDto>, DataShaper<EmployeeDto>>();
    builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.CacheProfiles.Add("120secondsDuration", new CacheProfile { Duration = 120 });
    }).AddXmlDataContractSerializerFormatters()
    .AddCustomCSVFormatter()
    .AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly);

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

}
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));


var app = builder.Build();
{
    var logger = app.Services.GetRequiredService<ILoggerManager>();
    app.ConfigureExceptionHandler(logger);

    if (app.Environment.IsProduction())
        app.UseHsts();  

    app.UseStaticFiles();

    app.UseIpRateLimiting();

    app.UseCors("CorsPolicy");

    app.UseResponseCaching();

    app.UseHttpCacheHeaders();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}