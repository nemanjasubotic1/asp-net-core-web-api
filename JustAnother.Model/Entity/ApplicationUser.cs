using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace JustAnother.Model.Entity;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string Name { get; set; }
}
