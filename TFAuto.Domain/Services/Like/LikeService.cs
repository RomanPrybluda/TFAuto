using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.LikeService
{
    public class LikeService : ILikeService
    {
        private readonly IRepository<Like> _repositoryLike;
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<TFAuto.DAL.Entities.User> _repositoryUser;

        public LikeService(IRepository<Like> repositoryLike,
            IRepository<Comment> repositoryComment,
            IRepository<TFAuto.DAL.Entities.User> repositoryUser)
        {
            _repositoryLike = repositoryLike;
            _repositoryComment = repositoryComment;
            _repositoryUser = repositoryUser;
        }

        public async ValueTask<bool> GiveLikeCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == commentId.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var existingLike = await _repositoryLike.GetAsync(l => l.CommentId == commentId.ToString() && l.UserId == userId.ToString()).FirstOrDefaultAsync();

            if (existingLike != null)
            {
                return false;
            }
            else
            {
                var like = new Like
                {
                    CommentId = commentId.ToString(),
                    UserId = userId.ToString()
                };

                await _repositoryLike.CreateAsync(like);

                comment.LikesCount++;
                await _repositoryComment.UpdateAsync(comment);

                return true;
            }
        }

        public async ValueTask<bool> RemoveLikeCommentAsync(Guid commentId, Guid userId)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == commentId.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var existingLike = await _repositoryLike.GetAsync(l => l.CommentId == commentId.ToString() && l.UserId == userId.ToString()).FirstOrDefaultAsync();

            if (existingLike == null)
            {
                return false;
            }
            else
            {
                await _repositoryLike.DeleteAsync(existingLike);

                comment.LikesCount--;
                await _repositoryComment.UpdateAsync(comment);

                return true;
            }
        }

        public async ValueTask<bool> RemoveLikesByCommentAsync(string commentId)
        {
            var likesOfComment = await _repositoryLike.GetAsync(l => l.CommentId == commentId);

            if (likesOfComment == null)
            {
                return false;
            }
            else
            {
                foreach (var like in likesOfComment)
                {
                    await _repositoryLike.DeleteAsync(like);
                }
                return true;
            }
        }

    }
}
