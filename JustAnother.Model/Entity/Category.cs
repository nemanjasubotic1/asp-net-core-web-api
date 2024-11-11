using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JustAnother.Model.Entity;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public IEnumerable<Movie> Movies { get; set; }
}
