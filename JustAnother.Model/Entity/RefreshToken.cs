using System.ComponentModel.DataAnnotations;

namespace JustAnother.Model.Entity;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; }
    public string JwtTokenId { get; set; }
    public string Refresh_Token { get; set; }
    public bool IsValid { get; set; }
    [DateInFuture]
    public DateTime ExipresAt { get; set; }
}
