using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.UserPassword.DTO
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}