using BlackGreenApi.Application.Services.Extensions;
using BlackGreenApi.Core.Models;
using BlackGreenApi.DataAccess.Data;
using BlackGreenApi.Presentation.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// ����������� EF Core � SQL Server
services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddOpenApi(options =>
{
	 options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_1;
});

services.AddEndpointsApiExplorer();
services.AddOpenApi();
//services.AddControllers;
services.AddControllersWithViews();
services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5099", "https://localhost:7143")
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials(); // �����!
    });
});

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));

// ����������� ��������
services.AddMyServices();

// ��������� ����������� ��� ������� � ������
services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFilter("BlackGreenApi", LogLevel.Information);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	 app.MapOpenApi();
	 app.MapControllers();
	 app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Open Api V1");
    });
}

// ��������� pipeline ��������� HTTP-��������
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// ��������� ������ ����� ��������� middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// ��������� ����������� ������ (CSS, JS, �����������)
app.UseDefaultFiles();
app.UseStaticFiles();

// ��������� HTTPS � �������������
app.UseHttpsRedirection();
app.UseRouting();

// ��������� CORS � �����������                          
app.UseCors("AllowLocalhost");
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.Run();