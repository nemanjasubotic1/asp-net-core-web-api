using JustAnother.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net;

namespace JustAnother.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/category")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private ApiResponse _apiResponse;
        public CategoryController()
        {
            _apiResponse = new();
        }

        [HttpGet]
        public IActionResult GetNames()
        {
            var names = new List<string>()
            {
                "nemanja", "sinko"
            };

            _apiResponse.IsSuccess = true;
            _apiResponse.Result = names;
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);

        }
    }
}
