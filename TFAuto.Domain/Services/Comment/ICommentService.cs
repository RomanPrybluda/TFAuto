using TFAuto.Domain.Services.CommentService.DTO;

namespace TFAuto.Domain.Services.CommentService
{
    public interface ICommentService
    {
        ValueTask<CreateCommentResponse> AddCommentAsync(CreateCommentRequest commentCreate);

        ValueTask<UpdateCommentResponse> UpdateCommentAsync(Guid id, UpdateCommentRequest commentUpdate);

        ValueTask DeleteCommentAsync(Guid id, DeleteCommentRequest commentDelete);

        ValueTask<GetAllCommentsResponse> GetAllCommentsAsync(Guid articleId, GetCommentsPaginationRequest paginationRequest);
    }
}
