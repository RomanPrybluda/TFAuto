using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Authentication.Models.Request;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; }
}
