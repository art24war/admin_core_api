using Newtonsoft.Json;
using ReactReduxApi.Models.Enums;

namespace ReactReduxApi.Models
{
    [JsonObject]
    public class LoginUserModel

    {
        public LoginUserModel()
        {
            Mode = "password";
        }

        [JsonProperty]
        public string Mode { get; set; } 

        [JsonProperty]
        public string Login { get; set; }

        [JsonProperty]
        public string Password { get; set; }

        [JsonProperty]
        public string RefreshToken { get; set; }
    }
}
