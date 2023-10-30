namespace TFAuto.Domain.Services.LikeService
{
    public interface ILikeService
    {
        ValueTask<bool> GiveLikeCommentAsync(Guid commentId, Guid userId);

        ValueTask<bool> RemoveLikeCommentAsync(Guid commentId, Guid userId);

        ValueTask<bool> RemoveLikesByCommentAsync(string commentId);
    }
}