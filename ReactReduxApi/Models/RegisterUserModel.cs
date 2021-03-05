namespace ReactReduxApi.Models
{
    public class RegisterUserModel
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; }
        public int? DealerId { get; set; }
        public string Comment { get; set; }
    }
}
