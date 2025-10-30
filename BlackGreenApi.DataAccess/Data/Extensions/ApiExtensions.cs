using BlackGreenApi.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security;
using System.Text;

namespace BlackGreenApi.DataAccess.Data.Extensions
{
	 public static class ApiExtensions
	 {
		  public static void AddApiAuthentication(this IServiceCollection services, IOptions<JwtOptions> jwtOptions)
		  {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
							AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
							options =>
							{
								 options.TokenValidationParameters = new()
								 {
									  ValidateIssuer = false,
									  ValidateAudience = false,
									  ValidateLifetime = true,
									  ValidateIssuerSigningKey = true,
									  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey))
								 };

								 options.Events = new JwtBearerEvents
								 {
									  OnMessageReceived = context =>
									  {
											context.Token = context.Request.Cookies["cok"];

											return Task.CompletedTask;
									  }
								 };
							});
				services.AddAuthorization();
		  }
	 }
}