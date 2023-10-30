using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Email.Models.Request;

public class ContactUsRequest
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string UserEmail { get; set; }

    [Required]
    [MaxLength(500)]
    public string Text { get; set; }
}
