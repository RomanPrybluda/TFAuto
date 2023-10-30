using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class CreateCommentRequest
    {
        [Required]
        public Guid ArticleId { get; set; }

        [Required]
        [MaxLength(500)]
        [MinLength(3)]
        public string Content { get; set; }

        [Required]
        public Guid AuthorId { get; set; }
    }
}