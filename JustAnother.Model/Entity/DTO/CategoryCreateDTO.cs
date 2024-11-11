using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JustAnother.Model.Entity.DTO;

public class CategoryCreateDTO
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }

    [JsonIgnore]
    public IEnumerable<Movie> Movies { get; set; }
}
