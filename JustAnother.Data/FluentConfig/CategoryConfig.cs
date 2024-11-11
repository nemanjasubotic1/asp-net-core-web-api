using JustAnother.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustAnother.DataAccess.FluentConfig;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasData(
            new Category { Id = 1, Name = "Action", Description = "Great action movies" },
            new Category { Id = 2, Name = "Adventure", Description = "Great epic movies" },
            new Category { Id = 3, Name = "Sci-Fi", Description = "Great sci-fi movies" }
            );
    }
}
