using AutoMapper;
using JustAnother.DataAccess.Repository.General;
using JustAnother.Model;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Net;
using System.Text.Json;

namespace JustAnother.API.Controllers.v1
{
    [Route("api/v{version:apiVersion}/category")]
    [ApiController]
    [ApiVersion("1.0")]
    public class CategoryController : ControllerBase
    {
        private ApiResponse _apiResponse;

        private readonly IUnitOfWork _unitOfWork;

        private readonly IMapper _autoMapper;
        public CategoryController(IUnitOfWork unitOfWork, IMapper autoMapper)
        {
            _apiResponse = new();
            _unitOfWork = unitOfWork;
            _autoMapper = autoMapper;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAllCategories([FromQuery] string? search, int pageSize = 1, int pageNumber = 1)
        {
            try
            {
                IEnumerable<Category> categoryList = await _unitOfWork.Category.GetAllAsync(pageNumber: pageNumber, pageSize: pageSize);

                if (!string.IsNullOrEmpty(search))
                {
                    categoryList = categoryList.Where(l => l.Name.ToLower().Contains(search));
                }

                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };


                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));

                _apiResponse.IsSuccess = true;
                _apiResponse.Result = categoryList;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = [ex.Message];

                return BadRequest(_apiResponse);
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategory(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;

                    return BadRequest(_apiResponse);
                }

                var category = await _unitOfWork.Category.GetAsync(l => l.Id == id);

                if (category == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(_apiResponse);
                }

                _apiResponse.IsSuccess = true;
                _apiResponse.Result = category;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = [ex.Message];
             
                return BadRequest(_apiResponse);
            }
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
                if (await _unitOfWork.Category.GetAsync(l => l.Name.ToLower() == categoryCreateDTO.Name.ToLower()) is not null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages = ["Category already exist."];

                    return BadRequest(_apiResponse);
                }

                if (categoryCreateDTO == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages = ["Input is null."];

                    return BadRequest(_apiResponse);
                }

                Category category = _autoMapper.Map<Category>(categoryCreateDTO);

                await _unitOfWork.Category.CreateAsync(category);

                await _unitOfWork.Save();

                _apiResponse.IsSuccess = true;
                _apiResponse.Result = category;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = [ex.Message];

                return BadRequest(_apiResponse);
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateCategory(int id, [FromForm] CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                if (id == 0 || id != categoryUpdateDTO.Id || categoryUpdateDTO == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    _apiResponse.ErrorMessages = ["Dont exist."];

                    return NotFound(_apiResponse);
                }

                Category category = _autoMapper.Map<Category>(categoryUpdateDTO);

                 _unitOfWork.Category.Update(category);

                await _unitOfWork.Save();

                _apiResponse.IsSuccess = true;
                _apiResponse.Result = category;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = [ex.Message];

                return BadRequest(_apiResponse);
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id == 0)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    _apiResponse.ErrorMessages = ["Dont exist."];

                    return BadRequest(_apiResponse);
                }

                var category = await _unitOfWork.Category.GetAsync(l => l.Id == id);

                if (category == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;

                    return NotFound(_apiResponse);
                }

                _unitOfWork.Category.Remove(category);

                await _unitOfWork.Save();

                _apiResponse.IsSuccess = true;
                _apiResponse.Result = category;
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.ErrorMessages = [ex.Message];

                return BadRequest(_apiResponse);
            }
        }
    }
}
