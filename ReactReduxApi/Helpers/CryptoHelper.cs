using DbModels;
using DbModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ReactReduxApi.Helpers
{
    public static class CryptoHelper
    {
        public static string GetSha256String(string password)
        {
            return Encoding.UTF8.GetString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        }

        public static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static object GetUserToken(UserModel user, SecurityTokenBuilder tokenBuilder, string refreshToken)
        {
            var token = tokenBuilder.BuildToken();
            return new
            {
                id = user.Id,
                userName = user.Login,
                token = tokenBuilder.BuildAccessToken(),
                expired = token.ValidTo,
                userType = user.UserType,
                refreshToken,
                roles = user.Roles?.Select(role => role.Role.RoleCode) ?? Array.Empty<string>()
            };
        }

        public static ClaimsIdentity GetUserClaims(UserModel user)
        {
            if (user.Roles == null)
                user.Roles = new List<UsersRoleRelation>();
            var claims = new ClaimsIdentity(new Claim[]
                                   {
                                    new Claim(ClaimTypes.Name, user.Login),
                                    new Claim("UserId", user.Id.ToString()),
                                    new Claim("UserType", user.UserType.ToString() )
                                   });
            switch (user.UserType)
            {
                case UserTypeEnum.Admin:
                case UserTypeEnum.Manager:
                    var typeName = user.UserType.GetDisplayName();
                    if (!user.Roles.Any(role => role.Role.RoleCode.Equals(typeName)))
                    {
                        user.Roles.Add(new UsersRoleRelation { Role = new RoleModel { RoleCode = typeName } });
                        claims.AddClaim(new Claim(ClaimTypes.Role, typeName));
                    }
                    break;
            }
            //Adding UserClaims to JWT claims
            foreach (var item in user.Roles ?? new List<UsersRoleRelation>())
            {
                claims.AddClaim(new Claim(ClaimTypes.Role, item.Role.RoleCode));
            }
            return claims;
        }
        public static ClaimsIdentity GetRefreshUserClaims(UserModel user)
        {
            return new ClaimsIdentity(new Claim[]
                                   {
                                    new Claim(ClaimTypes.Name, user.Login),
                                    new Claim("UserId", user.Id.ToString()),
                                    new Claim("UserType", user.UserType.ToString() )
                                   });
        }
    }
}
