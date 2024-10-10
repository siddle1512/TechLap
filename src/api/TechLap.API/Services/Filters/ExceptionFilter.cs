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
            var messgae = context.Exception.Message;
            var stackTrace = context.Exception.StackTrace;

            if (context.Exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
            }
            if (context.Exception is BadRequestException)
            {
                statusCode = StatusCodes.Status400BadRequest;
            }

            _logger.LogInformation("erroMsg: " + messgae);
            _logger.LogInformation("stackTrace: " + stackTrace?.ToString());

            context.Result = new JsonResult(new { Message = messgae, IsSuccess = false })
            {
                StatusCode = statusCode
            };
        }
    }
}
