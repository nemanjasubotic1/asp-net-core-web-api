using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JustAnother.Model.Entity;

public class Movie
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [MaxLength(300)]
    public string Description { get; set; }
    public bool IsOscarWinner { get; set; }
    public DateOnly InitialRelease { get; set; }

    [ForeignKey("Category")]
    public int CategoryId { get; set; }
    public Category Category { get; set; }

}
