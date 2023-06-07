using System.ComponentModel.DataAnnotations;
using Base.Domain;

namespace Domain.Identity;

public class RefreshToken : AbstractIdDatabaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    [MaxLength(64)] public string Token { get; set; } = Guid.NewGuid().ToString();

    public DateTime ExpiresAt { get; set; }

    [MaxLength(64)] public string? PreviousToken { get; set; }
    public DateTime? PreviousExpiresAt { get; set; }

    public RefreshToken() : this(7)
    {
    }

    public RefreshToken(double expiresInDays) : this(TimeSpan.FromDays(expiresInDays))
    {
    }

    public RefreshToken(TimeSpan expiresIn)
    {
        ExpiresAt = DateTime.UtcNow.AddMilliseconds(expiresIn.TotalMilliseconds);
    }

    public void Refresh(TimeSpan extendOldExpirationBy, TimeSpan expiresInDays)
    {
        PreviousToken = Token;
        PreviousExpiresAt = DateTime.UtcNow.AddMilliseconds(extendOldExpirationBy.TotalMilliseconds);

        Token = Guid.NewGuid().ToString();
        ExpiresAt = DateTime.UtcNow.AddMilliseconds(expiresInDays.TotalMilliseconds);
    }

    public bool IsExpired => ExpiresAt <= DateTime.UtcNow;

    public bool IsFullyExpired => IsExpired && PreviousExpiresAt != null && PreviousExpiresAt <= DateTime.UtcNow;
}