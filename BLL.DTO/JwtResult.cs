using Domain.Identity;

namespace BLL.DTO;

public class JwtResult
{
    public string Jwt { get; set; } = default!;
    public RefreshToken RefreshToken { get; set; } = default!;
}