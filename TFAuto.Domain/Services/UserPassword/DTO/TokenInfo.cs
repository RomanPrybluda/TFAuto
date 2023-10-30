namespace TFAuto.Domain.Services.UserPassword.DTO
{
    public class TokenInfo
    {
        public string UserId { get; set; }

        public DateTime Expiration { get; set; }
    }
}