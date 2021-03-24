using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ReactReduxApi.Helpers
{
    public class SecurityTokenBuilder
    {
        private IConfiguration _configuration;
        private string accessToken;
        private string encryptionKeyName;
        private ClaimsIdentity claims;
        private bool createNew;
        private string issuerKeyName;
        private string _audienceKeyName;
        private string _expiryTime;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private SecurityToken _securityToken;

        private DateTime _lastBuild;

        /// <summary>
        /// Creates security token
        /// </summary>
        public SecurityTokenBuilder()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }


        public SecurityTokenBuilder AddConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
            return this;
        }

        public SecurityTokenBuilder AddAccessToken(string token)
        {
            accessToken = token;
            return this;
        }

        public SecurityTokenBuilder AddEncriptionKey(string encription)
        {
            encryptionKeyName = encription;
            return this;
        }

        public SecurityTokenBuilder AddRefreshEncriptionKey(string encription)
        {
            encryptionKeyName = encription;
            return this;
        }

        public SecurityTokenBuilder AddIssuerKey(string key)
        {
            issuerKeyName = key;
            return this;
        }

        public SecurityTokenBuilder AddAudienceKey(string key)
        {
            _audienceKeyName = key;
            return this;
        }

        public SecurityTokenBuilder AddExpiryKey(string key)
        {
            _expiryTime = key;
            return this;
        }

        public SecurityTokenBuilder AddClaims(ClaimsIdentity claimsIdentity)
        {
            claims = claimsIdentity;
            return this;
        }

        public SecurityTokenBuilder SetCreateNew()
        {
            createNew = true;
            return this;
        }

        private void AssertParams()
        {
            Assert.IsNotNull(_configuration, nameof(_configuration));
            Assert.IsNotEmpty(encryptionKeyName, nameof(encryptionKeyName));
            Assert.IsNotEmpty(issuerKeyName, nameof(issuerKeyName));
            Assert.IsNotEmpty(_audienceKeyName, nameof(_audienceKeyName));
            Assert.IsNotEmpty(_expiryTime, nameof(_expiryTime));            
        }

        public SecurityToken BuildToken()
        {
            AssertParams();
            Assert.IsNotNull(claims, nameof(claims));
            
            var key = Encoding.ASCII.GetBytes(_configuration[encryptionKeyName]);
            _lastBuild = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration[issuerKeyName], //_configuration[Constants.JwtIssuer],
                Subject = claims,
                Audience = _configuration[_audienceKeyName], //_configuration[Constants.JwtAudience],
                //I have injected Configuration provider service into my controller
                Expires = _lastBuild.AddMinutes(Convert.ToDouble(_configuration[_expiryTime])), //Constants.JwtExpiryTime]
                //algorithm to sign the token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            _securityToken = _tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            return _securityToken;
        }

        public string BuildAccessToken()
        {
            var securityToken = createNew ? BuildToken() : _securityToken ?? BuildToken();
            createNew = false;
            return _tokenHandler.WriteToken(securityToken);
        }


        /// <summary>
        /// Validate access token 
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            Assert.IsNotEmpty(accessToken);
            AssertParams();
            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateAudience = false, // You might need to validate this one depending on your case
                ValidateIssuer = true,
                ValidIssuer = _configuration[issuerKeyName], // JwtIssuer
                ValidateActor = false,
                ValidateLifetime = false, // Do not validate lifetime here
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration[encryptionKeyName])) //Constants.JwtEncryptionKey
            };
;
            var principal = _tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out _securityToken);
            if (_securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token!");
            }

            var userId = principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException($"Missing claim: {ClaimTypes.Name}!");
            }

            return userId;
        }
    }
}
