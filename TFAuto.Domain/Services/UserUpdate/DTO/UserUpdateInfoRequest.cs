using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.UserUpdate.DTO
{
    public class UserUpdateInfoRequest
    {
        private string _userName;

        [Required]
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
        public string Email { get; set; }
    }
}