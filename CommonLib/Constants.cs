using System;

namespace CommonLib
{
    public static class Constants
    {
        public const string JwtCookieToken = "Jwt:CookieToken";
        public const string JwtExpiryTime = "Jwt:ExpiryTimeInMinutes";
        public const string JwtRefreshTokenExpiration = "Jwt:RefreshTokenExiration";
        public const string JwtEncryptionKey = "Jwt:EncriptionKey";
        public const string JwtRefreshEncriptionKey = "Jwt:RefreshEncriptionKey";
        public const string JwtIssuer = "Jwt:Issuer";
        public const string JwtAudience = "Jwt:Audience";
    }
}
