namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetAllCommentsResponse : BasePaginationResponse
    {
        public IEnumerable<GetCommentResponse> Comments { get; set; }
    }
}