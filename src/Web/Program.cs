using Educar.Backend.Application;
using Educar.Backend.Infrastructure;
using Educar.Backend.Infrastructure.Data;
using Educar.Backend.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices();
builder.Services.AddControllers();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5000", "http://127.0.0.1:5000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

var app = builder.Build();

// if (app.Environment.IsDevelopment()) 
await app.InitialiseDatabaseAsync();

app.UseHealthChecks("/health");

app.UseExceptionHandler(options => { });

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors(MyAllowSpecificOrigins);

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerUi(settings =>
{
    settings.Path = "/api";
    settings.DocumentPath = "/api/specification.json";
});

app.MapControllerRoute(
    "default",
    "{controller}/{action=Index}/{id?}");

//app.MapControllers();


app.Map("/", () => Results.Redirect("/api"));
app.MapEndpoints();

app.Run();