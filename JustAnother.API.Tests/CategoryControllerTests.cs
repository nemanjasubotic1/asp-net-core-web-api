using AutoMapper;
using JustAnother.API.Controllers.v1;
using JustAnother.DataAccess.Repository.CategoryRepo;
using JustAnother.DataAccess.Repository.General;
using JustAnother.Model.Entity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Linq.Expressions;
using System.Net;

namespace JustAnother.API.Tests;


[TestFixture]
public class CategoryControllerTests
{
    private Mock<IUnitOfWork> _mockUnitOfWork;
    private Mock<ICategoryRepository> _mockCategoryRepository;

    private CategoryController _controller;
    private Mock<IMapper> _mockAutoMapper;

    [SetUp]
    public void Setup()
    {
        _mockAutoMapper = new Mock<IMapper>();

        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();

        _controller = new CategoryController(_mockUnitOfWork.Object, _mockAutoMapper.Object);

    }

    [Test]
    public async Task GetCategory_ValidId_ReturnOkResult()
    {

        // Arrange
        var category = new Category { Id = 1, Name = "test", Description = "test" };

        _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

        _mockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
        .ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategory(1) as OkObjectResult;

        // Assert
        Assert.That(result, Is.Not.Null);
        ClassicAssert.AreEqual((int)HttpStatusCode.OK, result.StatusCode);

    }

    [Test]
    public async Task GetCategory_IdIsZero_ReturnsBadRequest()
    {
        // act
        var result = await _controller.GetCategory(0);

        // Assert
        ClassicAssert.IsInstanceOf<BadRequestObjectResult>(result);
        var badRequestResult = result as BadRequestObjectResult;
        ClassicAssert.AreEqual((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
    }

    [Test]
    public async Task GetCategory_WhenExceptionThrown_ReturnsBadRequest()
    {
        // arrange
        _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

        _mockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
         .ThrowsAsync(new Exception());

        // act
        var result = await _controller.GetCategory(1) as BadRequestObjectResult;

        // assert
        Assert.That(result, Is.Not.Null);
        ClassicAssert.AreEqual((int)HttpStatusCode.BadRequest, result.StatusCode);
    }


    [Test]
    public async Task GetCategory_CategoryVerifyNumberOfCalls_ReturnsVerifyResult()
    {
        // arrange
        var category = new Category { Id = 1, Name = "test", Description = "test" };

        _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

        _mockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
        .ReturnsAsync(category);

        // act
        var result = await _controller.GetCategory(1);


        // assert
        Assert.That(result, Is.Not.Null);

        _mockUnitOfWork.Verify(u => u.Category, Times.Once);
        _mockCategoryRepository.Verify(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1), Times.Once);

    }

    [Test]
    public async Task GetCategory_CategoryNotFound_ReturnsNotFound()
    {
        // arrange
        _mockUnitOfWork.Setup(u => u.Category).Returns(_mockCategoryRepository.Object);

        _mockCategoryRepository.Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), true, null, 0, 1))
            .ReturnsAsync(null as Category);


        // act
        var result = await _controller.GetCategory(1);

        // assert
        ClassicAssert.IsInstanceOf<NotFoundObjectResult>(result);
        var notFoundResult = result as NotFoundObjectResult;
        Assert.That((int)HttpStatusCode.NotFound, Is.EqualTo(notFoundResult.StatusCode));

    }
}
