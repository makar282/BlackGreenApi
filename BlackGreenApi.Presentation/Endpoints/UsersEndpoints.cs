using BlackGreenApi.Application.Services;
using BlackGreenApi.Core.Models;

namespace BlackGreenApi.Presentation.Endpoints
{
    public static class UsersEndpoints
    {
        public static async Task<IResult> Register(UserService userService, RegisterUserRequest request)
        {
            try
            {
                await userService.Register(request.UserName, request.Password);
                return Results.Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        public static async Task<IResult> Login(UserService userService, LoginUserRequest request, HttpContext httpContext)
        {
            try
            {
                var token = await userService.Login(request.UserName, request.Password);

                var isDevelopment = httpContext.Request.Host.Host.Contains("localhost");

                httpContext.Response.Cookies.Append("MyCookie", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                });


                return Results.Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }
    }
}