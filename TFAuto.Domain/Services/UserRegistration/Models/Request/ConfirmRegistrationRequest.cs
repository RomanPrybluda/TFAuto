using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain;

public class ConfirmRegistrationRequest
{
    private string _userName;
    [Required]
    [DefaultValue("VasiaVasiliev")]
    [RegularExpression(@"^[A-Za-z]+$", ErrorMessage = ErrorMessages.USER_VALID_USER_NAME)]
    [MaxLength(50, ErrorMessage = ErrorMessages.USER_VALID_USER_NAME_LENGTH)]
    public string UserName
    {
        get { return _userName; }
        set
        {
            if (value.Length > 0)
            {
                _userName = char.ToUpper(value[0]) + value.Substring(1);
            }
            else
            {
                _userName = value;
            }
        }
    }

    [Required]
    [EmailAddress]
    [MaxLength(50, ErrorMessage = ErrorMessages.USER_VALID_EMAIL)]
    public string Email { get; set; }

    [Required]
    [DefaultValue("Qwerty123!")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%*^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    public string Password { get; set; }

    [Required]
    [DefaultValue("Qwerty123!")]
    [Compare(nameof(Password), ErrorMessage = ErrorMessages.USER_VALID_REPEAT_PASSWORD)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%*^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
    public string RepeatPassword { get; set; }

}
