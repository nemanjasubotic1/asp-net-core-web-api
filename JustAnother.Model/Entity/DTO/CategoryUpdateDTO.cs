using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace JustAnother.Model.Entity.DTO
{
    public class CategoryUpdateDTO
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public IEnumerable<Movie> Movies { get; set; }
    }
}
