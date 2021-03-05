using Newtonsoft.Json;

namespace ReactReduxApi.Models
{
    [JsonObject]
    public class LoginUserModel
    {
        [JsonProperty]
        public string Login { get; set; }

        [JsonProperty]
        public string Password { get; set; }
    }
}
