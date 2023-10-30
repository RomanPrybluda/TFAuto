using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.Services.CommentService.DTO;
using TFAuto.Domain.Services.LikeService;

namespace TFAuto.Domain.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<Article> _repositoryArticle;
        private readonly IRepository<TFAuto.DAL.Entities.User> _repositoryUser;
        private readonly IRepository<Role> _repositoryRole;
        private readonly IMapper _mapper;
        private readonly ILikeService _likeService;

        public CommentService(
            IRepository<Comment> repositoryComment,
            IRepository<Article> repositoryArticle,
            IRepository<TFAuto.DAL.Entities.User> repositoryUser,
            IRepository<Role> repositoryRole,
            IMapper mapper,
            ILikeService likeService)
        {
            _repositoryComment = repositoryComment;
            _repositoryArticle = repositoryArticle;
            _repositoryUser = repositoryUser;
            _repositoryRole = repositoryRole;
            _mapper = mapper;
            _likeService = likeService;
        }

        public async ValueTask<CreateCommentResponse> AddCommentAsync(CreateCommentRequest commentCreate)
        {
            var article = await _repositoryArticle.GetAsync(t => t.Id == commentCreate.ArticleId.ToString()).FirstOrDefaultAsync();

            if (article == null)
                throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentCreate.AuthorId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var commentMapped = _mapper.Map<Comment>(commentCreate);
            commentMapped.ArticleId = article.Id;
            var createdComment = await _repositoryComment.CreateAsync(commentMapped);

            var createdCommentResponce = _mapper.Map<CreateCommentResponse>(createdComment);

            return createdCommentResponce;
        }

        public async ValueTask<UpdateCommentResponse> UpdateCommentAsync(Guid id, UpdateCommentRequest commentUpdate)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == id.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentUpdate.UserId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            if (user.Id != comment.AuthorId)
                throw new ValidationException(ErrorMessages.USER_IS_NOT_COMMENT_AUTHOR);

            var articleId = comment.ArticleId;
            var article = await _repositoryArticle.GetAsync(t => t.Id == articleId).FirstOrDefaultAsync();

            if (article == null)
                throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

            var commentMapped = _mapper.Map<Comment>(commentUpdate);
            var updatedComment = await _repositoryComment.UpdateAsync(commentMapped);

            updatedComment.ArticleId = article.Id;
            var updatedCommentResponce = _mapper.Map<UpdateCommentResponse>(updatedComment);

            return updatedCommentResponce;
        }

        public async ValueTask DeleteCommentAsync(Guid id, DeleteCommentRequest commentDelete)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == id.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentDelete.UserId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role.RoleName == RoleNames.SUPER_ADMIN)
            {
                await _repositoryComment.DeleteAsync(comment);
                await _likeService.RemoveLikesByCommentAsync(comment.Id);
            }
            else
            {
                var article = await _repositoryArticle.GetAsync(t => t.Id == comment.ArticleId.ToString()).FirstOrDefaultAsync();

                if (user.Id != article.UserId)
                    throw new ValidationException(ErrorMessages.USER_IS_NOT_ARTICLE_AUTHOR);

                await _repositoryComment.DeleteAsync(comment);
                await _likeService.RemoveLikesByCommentAsync(comment.Id);
            }
        }

        public async ValueTask<GetAllCommentsResponse> GetAllCommentsAsync(Guid articleId, GetCommentsPaginationRequest paginationRequest)
        {

            var article = await _repositoryArticle.GetAsync(t => t.Id == articleId.ToString()).FirstOrDefaultAsync();

            if (article == null)
                throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

            string queryComments = $"SELECT * FROM c WHERE c.type = \"{nameof(Comment)}\" AND c.articleId = \"{articleId.ToString()}\" ORDER BY c.createdTimeUtc DESC"; ;
            var commentList = await _repositoryComment.GetByQueryAsync(queryComments);

            if (commentList == null)
                throw new NotFoundException(ErrorMessages.COMMENTS_NOT_FOUND);

            var totalItems = commentList.Count();

            if (totalItems <= paginationRequest.Skip)
                throw new NotFoundException(ErrorMessages.COMMENTS_NOT_FOUND);

            if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
                paginationRequest.Take = (totalItems - paginationRequest.Skip);

            var commentsResponseList = new List<GetCommentResponse>();

            foreach (var comment in commentList)
            {
                var author = await _repositoryUser.GetAsync(u => u.Id == comment.AuthorId).FirstOrDefaultAsync();

                if (author != null)
                {
                    var commentResponse = new GetCommentResponse
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        LikesCount = comment.LikesCount,
                        AuthorCommentName = author.UserName,
                        CreatedAt = comment.CreatedTimeUtc.GetValueOrDefault(),
                        ArticleId = comment.ArticleId
                    };
                    commentsResponseList.Add(commentResponse);
                }
            }

            var allCommentsResponse = new GetAllCommentsResponse()
            {
                TotalItems = totalItems,
                Skip = paginationRequest.Skip,
                Take = paginationRequest.Take,
                Comments = commentsResponseList
            };

            return allCommentsResponse;
        }

    }
}