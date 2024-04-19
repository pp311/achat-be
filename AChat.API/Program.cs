using System.Text.Json.Serialization;
using FluentValidation;
using Serilog;
using Serilog.Events;
using AChat.Application.Services;
using AChat.Extensions;
using AChat.Middlewares;
using AChat.SignalRHub;
using IRequest = AChat.Application.ViewModels.IRequest;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();
builder.AddSettings();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .ConfigureCors()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureIdentity()
    .AddSwagger("AChat API v1")
    .AddRepositories()
    .AddServices()
    .AddCurrentUser()
    .AddQuartz()
    .AddMemoryCache()
    .AddAutoMapper(typeof(BaseService).Assembly)
    .AddProblemDetails()
    .AddExceptionHandler<GlobalExceptionHandler>()
    .AddAuthentication(builder.Configuration)
    .AddValidatorsFromAssemblyContaining<IRequest>(ServiceLifetime.Singleton)
    .AddSentry();

builder.Services.AddSignalR();

var app = builder.Build();

await app.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization","true");
    });
}

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
// app.UseHttpsRedirection();

app.MapControllers();
app.MapHub<SignalRHub>("/signalr");

app.Run();
