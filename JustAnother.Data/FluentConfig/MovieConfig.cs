using JustAnother.Model.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JustAnother.DataAccess.FluentConfig;

public class MovieConfig : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasData(
            new Movie { 
                Id = 1, 
                Name = "Action Movie 1", 
                Description = "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs." ,
                IsOscarWinner = true,
                InitialRelease = new DateOnly(2020,01,01),
                CategoryId = 1,
            },
             new Movie
             {
                 Id = 2,
                 Name = "Adventure Movie 1",
                 Description = "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
                 IsOscarWinner = false,
                 InitialRelease = new DateOnly(1986, 05, 05),
                 CategoryId = 2,
             },
             new Movie
             {
                 Id = 3,
                 Name = "SciFi Movie 1",
                 Description = "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
                 IsOscarWinner = false,
                 InitialRelease = new DateOnly(1960, 05, 05),
                 CategoryId = 3,
             },
             new Movie
             {
                 Id = 4,
                 Name = "Adventure Movie 2",
                 Description = "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
                 IsOscarWinner = true,
                 InitialRelease = new DateOnly(1986, 09, 05),
                 CategoryId = 2,
             },
             new Movie
             {
                 Id = 5,
                 Name = "Action Movie 2",
                 Description = "Lorem ipsum, or lipsum as it is sometimes known, is dummy text used in laying out print, graphic or web designs.",
                 IsOscarWinner = false,
                 InitialRelease = new DateOnly(2003, 05, 05),
                 CategoryId = 1,
             }

            );
    }
}
