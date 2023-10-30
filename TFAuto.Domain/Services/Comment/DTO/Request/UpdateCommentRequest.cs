using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class UpdateCommentRequest
    {
        [Required]
        [MaxLength(2500)]
        [MinLength(3)]
        public string Content { get; set; }

        [Required]
        public Guid UserId { get; set; }
    }
}