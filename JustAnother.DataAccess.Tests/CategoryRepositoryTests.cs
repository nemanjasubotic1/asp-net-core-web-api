using JustAnother.DataAccess.Data;
using JustAnother.DataAccess.Repository.CategoryRepo;
using JustAnother.Model.Entity;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections;

namespace JustAnother.DataAccess.Tests;

[TestFixture]
public class CategoryRepositoryTests
{
    private DbContextOptions<ApplicationDbContext> options;

    private Category categoryOne;
    private Category categoryTwo;

    [SetUp]
    public void Setup()
    {
        categoryOne = new()
        {
            Id = 1,
            Name = "One",
            Description = "Description of the category one"
        };

        categoryTwo = new()
        {
            Id = 2,
            Name = "Two",
            Description = "Description of the category two"
        };

        options = new DbContextOptionsBuilder<ApplicationDbContext>().
        UseInMemoryDatabase(databaseName: "temp_another_database").Options;

    }

    [Test]
    [Order(1)]
    public async Task AddCategory_InputCategoryOne_CheckDatabaseValues()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>().
            UseInMemoryDatabase(databaseName: "temp_another_database").Options;

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CategoryRepository(context);

            await repository.CreateAsync(categoryOne);

            await context.SaveChangesAsync();
        }

        using (var context = new ApplicationDbContext(options))
        {
            var repositoryForDb = context.Categories.FirstOrDefault(l => l.Id == 1);

            ClassicAssert.AreEqual(categoryOne.Id, repositoryForDb.Id);
            ClassicAssert.AreEqual(categoryOne.Name, repositoryForDb.Name);
            ClassicAssert.AreEqual(categoryOne.Description, repositoryForDb.Description);
        }
    }

    [Test]
    [Order(2)]
    public async Task GetAllCategories_CategoryOneAndTwo_CheckDatabaseValues()
    {
        // arrange
        var expectedResult = new List<Category> { categoryOne, categoryTwo };

        using (var context = new ApplicationDbContext(options))
        {
            await context.Database.EnsureDeletedAsync();

            var repository = new CategoryRepository(context);

            await repository.CreateAsync(categoryOne);
            await repository.CreateAsync(categoryTwo);

            await context.SaveChangesAsync();
        }

        // act
        List<Category> actuallListFromDb;

        using (var context = new ApplicationDbContext(options))
        {
            var repository = new CategoryRepository(context);

            actuallListFromDb = await repository.GetAllAsync();

        }

        // assert
        CollectionAssert.AreEqual(expectedResult, actuallListFromDb, new CategoryComparer());


    }

    private class CategoryComparer : IComparer
    {
        public int Compare(object? x, object? y)
        {
            var category1 = (Category)x!;
            var category2 = (Category)y!;

            if (category1.Id != category2.Id)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }


}
