using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TechLap.API.Exceptions;

namespace TechLap.API.Services.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private ILogger _logger;
        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            var statusCode = StatusCodes.Status500InternalServerError;

            if (context.Exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }

            _logger.LogInformation("erroMsg: " + context.Exception.Message);
            _logger.LogInformation("stackTrace: " + context.Exception.StackTrace?.ToString());

            context.Result = new JsonResult(new { IsSuccess = false })
            {
                StatusCode = statusCode
            };
        }
    }
}
