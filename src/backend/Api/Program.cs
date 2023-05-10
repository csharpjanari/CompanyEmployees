using Api.Extensions;
using Contracts;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Presentation.ActionFilters;

var builder = WebApplication.CreateBuilder(args);
{
    LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

    builder.Services.ConfigureCors();
    builder.Services.ConfigureLoggerService();
    builder.Services.ConfigureRepositoryManager();
    builder.Services.ConfigureServiceManager();
    builder.Services.ConfigureSqlContext(builder.Configuration);
    builder.Services.AddAutoMapper(typeof(Program));

    builder.Services.AddScoped<ValidationFilterAttribute>();
    builder.Services.AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
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

    app.UseCors("CorsPolicy");

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
    ThreadStart ci for api..
}