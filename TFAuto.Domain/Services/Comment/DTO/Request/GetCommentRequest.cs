using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetCommentRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CurrentPage { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }
    }
}