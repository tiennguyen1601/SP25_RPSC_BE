using System;
using System.Collections.Generic;

namespace SP25_RPSC.Data.Entities;

public partial class RefreshToken
{
    public string RefreshTokenId { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public string Token { get; set; } = null!;

    public string? UserId { get; set; }

    public virtual User? User { get; set; }
}
