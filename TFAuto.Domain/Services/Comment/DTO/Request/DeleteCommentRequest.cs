using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class DeleteCommentRequest
    {
        [Required]
        public Guid UserId { get; set; }
    }
}