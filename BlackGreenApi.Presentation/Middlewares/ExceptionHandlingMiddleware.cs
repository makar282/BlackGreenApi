namespace BlackGreenApi.Presentation.Middlewares
{
	 public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
	 {
		  private readonly RequestDelegate _next = next;
		  private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

		  public async Task InvokeAsync(HttpContext context)
		  {
				try
				{
					 await _next(context);
				}
				catch (Exception ex)
				{
					 _logger.LogError(ex, "Exeption occured: {Message}", ex.Message);
					 context.Response.StatusCode = StatusCodes.Status500InternalServerError;

					 await context.Response.WriteAsJsonAsync(ex.Message);
				}
		  }
	 }
}
