using DbModels;

namespace DbRepository.Repositories.Models
{
    /// <summary>
    /// Response from login server
    /// </summary>
    public class LoginResponse
    {
        public UserModel User { get; set; }
        public bool LoginResult { get; set; }
        public string RefreshToken { get; set; }
    }
}
