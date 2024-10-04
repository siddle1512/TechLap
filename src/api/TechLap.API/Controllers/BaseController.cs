using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TechLap.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        protected IActionResult CreateResponse<Response>(bool isSuccess, string message, HttpStatusCode statusCode, Response? data = default)
        {
            var apiResponse = new ApiResponse<Response>
            {
                IsSuccess = isSuccess,
                Data = data,
                Message = "Request processed successfully."
            };
            return StatusCode((int)statusCode, apiResponse);
        }
    }
}
