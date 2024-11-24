using AutoMapper;
using JustAnother.API.Controllers.v1;
using JustAnother.DataAccess.Repository.CategoryRepo;
using JustAnother.DataAccess.Repository.General;
using JustAnother.Model;
using JustAnother.Model.Entity;
using JustAnother.Model.Entity.DTO;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Linq.Expressions;
using System.Net;
using Xunit;

namespace JustAnother.API.Tests;

public class CategoryControllerXUnitTests
{
    private Mock<ICategoryRepository> _mockCategoryRepository;
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<IMapper> _mockAutoMapper;

    private CategoryController _controller;
    private ApiResponse _response;

    public CategoryControllerXUnitTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAutoMapper = new Mock<IMapper>();

        _controller = new CategoryController(_mockUnitOfWork.Object, _mockAutoMapper.Object);


        _mockUnitOfWork.Setup(l => l.Category).Returns(_mockCategoryRepository.Object);

        _response = new()
        {
            IsSuccess = false,
            StatusCode = HttpStatusCode.BadRequest
        };

    }

    [Fact]
    public async Task GetCategory_InputId_ShuoldReturnOkObjectResult()
    {
        // arrange
        var category = new Category { Id = 1, Name = "Name" };

        _mockCategoryRepository.Setup(l => l.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
            .ReturnsAsync(category);

        // act
        var result = await _controller.GetCategory(1);

        // assert

        Assert.NotNull(result);
        var okObjectResult = Assert.IsType<OkObjectResult>(result);

        Assert.NotNull(okObjectResult);

        var returnedApiResponse = Assert.IsType<ApiResponse>(okObjectResult.Value);

        var returnedCategory = Assert.IsType<Category>(returnedApiResponse.Result);
        Assert.Equal(category.Id, returnedCategory.Id);
    }

    [Fact]
    public async Task GetCategory_ThrowsException()
    {
        // arrange
        _mockCategoryRepository.Setup(l => l.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
            .ThrowsAsync(new Exception());

        // act
        var result = await _controller.GetCategory(1);

        // assert
        Assert.NotNull(result);

        var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);

        var returnedApiResponse = Assert.IsType<ApiResponse>(badRequestObjectResult.Value);

        Assert.Equal(_response.IsSuccess, returnedApiResponse.IsSuccess);
        Assert.Equal(_response.StatusCode, returnedApiResponse.StatusCode);

    }

    [Fact]
    public async Task CreateCategory_InputCategoryDTO_ShuoldReturnOkObjectResult()
    {
        // arrange
        CategoryCreateDTO categoryCreateDto = new CategoryCreateDTO { Name = "Test" };
        Category expectedCategory = new Category { Id = 1, Name = "Test" };

        _mockCategoryRepository.Setup(l => l.CreateAsync(It.IsAny<Category>()));

        _mockAutoMapper.Setup(m => m.Map<Category>(It.IsAny<CategoryCreateDTO>()))
                .Returns((CategoryCreateDTO createDto) => new Category { Id = 1, Name = createDto.Name });

        // act
        var result = await _controller.CreateCategory(categoryCreateDto);

        // assert
        Assert.NotNull(result);

        var okObjectResult = Assert.IsType<OkObjectResult>(result);

        var returnedApiResponse = Assert.IsType<ApiResponse>(okObjectResult.Value);
        Assert.NotEqual(_response.IsSuccess, returnedApiResponse.IsSuccess);

        var createdCategory = Assert.IsType<Category>(returnedApiResponse.Result);
        Assert.Equal(expectedCategory.Name, createdCategory.Name);

    }
}
