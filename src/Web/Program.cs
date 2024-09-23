using Educar.Backend.Application;
using Educar.Backend.Infrastructure;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();

var app = builder.Build();

// if (app.Environment.IsDevelopment()) 
await app.InitialiseDatabaseAsync();

app.UseHealthChecks("/health");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

app.UseExceptionHandler(options => { });

app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints();

app.Run();