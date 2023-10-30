namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class CreateCommentResponse
    {
        public string Id { get; set; }

        public string ArticleId { get; set; }

        public string AuthorId { get; set; }

        public string Content { get; set; }
    }
}