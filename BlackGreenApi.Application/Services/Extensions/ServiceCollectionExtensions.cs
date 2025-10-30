using BlackGreenApi.Application.Repositories;
using BlackGreenApi.Application.Services.Interfaces;


namespace BlackGreenApi.Application.Services.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyServices(this IServiceCollection services)
        {
            // User-related services
            services.AddHttpClient<IUserService, UserService>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher, PasswordHasherService>();

            // Other services
            //services.AddScoped<IReceiptApi, ReceiptApiService>();
            //services.AddScoped<IRecommendationMatcher, RecommendationMatcherService>();
            //services.AddScoped<IParserJson, ParserJsonService>();

            return services;
        }
    }
}
