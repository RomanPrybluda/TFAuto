using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.UserRegistration.Models.Request;

public class RegistrationRequest
{
    [Required]
    public string Id { get; set; }

    [Required]
    public string Token { get; set; }
}