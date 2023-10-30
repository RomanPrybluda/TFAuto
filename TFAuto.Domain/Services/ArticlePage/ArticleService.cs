using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.ExtensionMethods;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;
using TFAuto.Domain.Services.Authentication.Constants;
using TFAuto.Domain.Services.Blob;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.ArticlePage;
public class ArticleService : IArticleService
{
    private readonly IRepository<Article> _repositoryArticle;
    private readonly IRepository<User> _repositoryUser;
    private readonly IRepository<Tag> _repositoryTag;
    private readonly IRepository<Comment> _repositoryComment;
    private readonly IBlobService _imageService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMapper _mapper;

    public ArticleService(IRepository<Article> repositoryArticle, IRepository<User> repositoryUser, IRepository<Tag> repositoryTag, IBlobService imageService, IHttpContextAccessor contextAccessor, IMapper mapper, IRepository<Comment> repositoryComment)
    {
        _repositoryArticle = repositoryArticle;
        _repositoryUser = repositoryUser;
        _repositoryTag = repositoryTag;
        _repositoryComment = repositoryComment;
        _imageService = imageService;
        _contextAccessor = contextAccessor;
        _mapper = mapper;
    }

    public async ValueTask<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest articleRequest)
    {
        var articleAuthorId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;
        var articleAuthorName = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_NAME)?.Value;

        if (articleAuthorId == null || articleAuthorName == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_NOT_FOUND);

        Article articleEntityFromRequest = _mapper.Map<Article>(articleRequest);

        var articleAuthorEntity = await _repositoryUser.GetAsync(c => c.Id == articleAuthorId).FirstOrDefaultAsync();

        if (articleAuthorEntity == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_NOT_FOUND);

        articleAuthorEntity.ArticleIds.Add(articleEntityFromRequest.Id);
        await _repositoryUser.UpdateAsync(articleAuthorEntity);

        List<Tag> tagsForArticleEntityList = await AllocateTags(articleRequest.Tags, articleEntityFromRequest);

        var imageResponse = await _imageService.UploadAsync(articleRequest.Image);
        articleEntityFromRequest.UserId = articleAuthorId;
        articleEntityFromRequest.UserName = articleAuthorName;
        articleEntityFromRequest.LastUserWhoUpdated = articleAuthorName;
        articleEntityFromRequest.ImageFileName = imageResponse.FileName;

        var dataArticleEntity = await _repositoryArticle.CreateAsync(articleEntityFromRequest);

        CreateArticleResponse articleResponse = _mapper.Map<CreateArticleResponse>(dataArticleEntity);
        articleResponse.Image = imageResponse;

        foreach (Tag tag in tagsForArticleEntityList)
        {
            TagResponse tagResponse = _mapper.Map<TagResponse>(tag);
            articleResponse.Tags.Add(tagResponse);
        }

        return articleResponse;
    }

    public async ValueTask<UpdateArticleResponse> UpdateArticleAsync(Guid articleId, UpdateArticleRequest articleRequest)
    {
        var lastArticleAuthorName = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_NAME)?.Value;

        if (lastArticleAuthorName == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_WHO_UPDATED_NOT_FOUND);

        Article articleEntityFromRequest = _mapper.Map<Article>(articleRequest);

        var existingArticleEntity = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (existingArticleEntity == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        foreach (var existingArticleEntityTagId in existingArticleEntity.TagIds)
        {
            var tagUpdatedByArticle = await _repositoryTag.GetAsync(c => c.Id == existingArticleEntityTagId).FirstOrDefaultAsync();
            tagUpdatedByArticle.ArticleIds.Remove(existingArticleEntity.Id);
            await _repositoryTag.UpdateAsync(tagUpdatedByArticle);
        }

        existingArticleEntity.TagIds.Clear();

        List<Tag> newArticleEntityTagsList = await AllocateTags(articleRequest.Tags, existingArticleEntity);

        var imageResponse = await _imageService.UpdateAsync(existingArticleEntity.ImageFileName, articleRequest.Image);

        existingArticleEntity.Name = articleEntityFromRequest.Name;
        existingArticleEntity.Text = articleEntityFromRequest.Text;
        existingArticleEntity.LastUserWhoUpdated = lastArticleAuthorName;
        existingArticleEntity.ImageFileName = imageResponse.FileName;

        var dataArticle = await _repositoryArticle.UpdateAsync(existingArticleEntity);

        UpdateArticleResponse articleResponse = _mapper.Map<UpdateArticleResponse>(dataArticle);

        foreach (Tag tag in newArticleEntityTagsList)
        {
            TagResponse tagResponse = _mapper.Map<TagResponse>(tag);
            articleResponse.Tags.Add(tagResponse);
        }

        articleResponse.Image = imageResponse;

        return articleResponse;
    }

    public async ValueTask<GetSingleArticleResponse> GetArticleAsync(Guid articleId)
    {
        var article = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (article == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        var userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;
        bool IsLikedByUser = false;
        if (article.LikedUserIds.Contains(userId))
        {
            IsLikedByUser = true;
        }

        string queryTagsByArticleId = $"SELECT * FROM c WHERE c.type = \"{nameof(Tag)}\" AND ARRAY_CONTAINS(c.{nameof(Tag.ArticleIds).FirstLetterToLower()}, '{article.Id}')";
        var tagsList = await _repositoryTag.GetByQueryAsync(queryTagsByArticleId);

        string queryCommentsQuantity = $"SELECT * FROM c WHERE c.type = \"{nameof(Comment)}\" AND c.{nameof(Comment.ArticleId).FirstLetterToLower()} = \"{article.Id}\"";
        var commentsList = await _repositoryComment.GetByQueryAsync(queryCommentsQuantity);

        var imageResponse = await _imageService.GetAsync(article.ImageFileName);

        GetSingleArticleResponse articleResponse = _mapper.Map<GetSingleArticleResponse>(article);
        articleResponse.Image = imageResponse;
        articleResponse.Tags = tagsList.Select(tag => _mapper.Map<TagResponse>(tag)).ToList();
        articleResponse.CommentsCount = commentsList.Count();
        articleResponse.IsLikedByUser = IsLikedByUser;

        return articleResponse;
    }

    public async ValueTask<GetAllArticlesResponse> GetAllArticlesAsync(GetAllArticlesRequest paginationRequest, string userWhoLikedPages = "")
    {
        string queryArticles = await BuildQuery(paginationRequest, userWhoLikedPages);
        var articleList = await _repositoryArticle.GetByQueryAsync(queryArticles);

        if (articleList == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        var totalItems = articleList.Count();

        if (totalItems <= paginationRequest.Skip)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
            paginationRequest.Take = (totalItems - paginationRequest.Skip);

        var articlesResponse = articleList
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .Select(async article => await ConvertGetArticleResponse(article))
            .WhenAll().ToList();

        var allArticlesResponse = new GetAllArticlesResponse()
        {
            TotalItems = totalItems,
            Skip = paginationRequest.Skip,
            Take = paginationRequest.Take,
            OrderBy = paginationRequest.SortBy,
            Articles = articlesResponse
        };

        return allArticlesResponse;
    }

    public async ValueTask<bool> SetLikeAsync(Guid articleId)
    {
        var userWhoLikes = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;

        if (userWhoLikes == null)
            throw new ValidationException(ErrorMessages.LIKE_USER_NOT_PERMITTED);

        var article = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (article == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        if (article.LikedUserIds.Any(c => c == userWhoLikes))
        {
            return false;
        }
        else
        {
            article.LikedUserIds.Add(userWhoLikes);
            article.LikesCount = article.LikedUserIds.Count;
            await _repositoryArticle.UpdateAsync(article);

            var user = await _repositoryUser.GetAsync(c => c.Id == userWhoLikes).FirstOrDefaultAsync();
            user.LikedArticleIds.Add(article.Id);
            await _repositoryUser.UpdateAsync(user);

            var userWhoWasLiked = await _repositoryUser.GetAsync(c => c.Id == article.UserId).FirstOrDefaultAsync();
            userWhoWasLiked.ReceivedLikesFromUserId.Add(user.Id);
            await _repositoryUser.UpdateAsync(userWhoWasLiked);

            return true;
        }
    }

    public async ValueTask<bool> RemoveLikeAsync(Guid articleId)
    {
        var userWhoRemovesLike = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;

        if (userWhoRemovesLike == null)
            throw new ValidationException(ErrorMessages.LIKE_USER_NOT_PERMITTED);

        var article = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (article == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        if (article.LikedUserIds.Any(c => c == userWhoRemovesLike))
        {
            article.LikedUserIds.Remove(userWhoRemovesLike);
            article.LikesCount = article.LikedUserIds.Count;
            await _repositoryArticle.UpdateAsync(article);

            var user = await _repositoryUser.GetAsync(c => c.Id == userWhoRemovesLike).FirstOrDefaultAsync();
            user.LikedArticleIds.Remove(article.Id);
            await _repositoryUser.UpdateAsync(user);

            var userWhoWasDisliked = await _repositoryUser.GetAsync(c => c.Id == article.UserId).FirstOrDefaultAsync();
            userWhoWasDisliked.ReceivedLikesFromUserId.Remove(user.Id); ;
            await _repositoryUser.UpdateAsync(userWhoWasDisliked);

            return true;
        }
        else
        {
            return false;
        }
    }

    public async ValueTask<GetTopAuthorsResponse> GetTopAuthorsAsync(GetTopAuthorsRequest paginationRequest)
    {
        string baseQuery =
            $"SELECT * FROM c WHERE c.type = \"{nameof(User)}\" " +
            $"AND( c.{nameof(User.RoleId).FirstLetterToLower()} = \"{RoleId.AUTHOR}\" OR c.{nameof(User.RoleId).FirstLetterToLower()} = \"{RoleId.SUPER_ADMIN}\")";

        var authorsList = await _repositoryUser.GetByQueryAsync(baseQuery);

        if (authorsList == null)
            throw new NotFoundException(ErrorMessages.AUTHOR_NOT_EXISTS);

        var authorsWithArticles = authorsList.Where(c => c.ArticleIds.Count > 0);
        var totalItems = authorsWithArticles.Count();

        if (totalItems <= paginationRequest.Skip)
            throw new NotFoundException(ErrorMessages.AUTHOR_NOT_EXISTS);

        if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
            paginationRequest.Take = (totalItems - paginationRequest.Skip);

        var authorsResponse = authorsWithArticles
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .OrderByDescending(c => c.ReceivedLikesFromUserId.Count)
            .Select(authors => _mapper.Map<GetAuthorResponse>(authors))
            .ToList();

        var topAuthorsResponse = new GetTopAuthorsResponse()
        {
            TotalItems = totalItems,
            Skip = paginationRequest.Skip,
            Take = paginationRequest.Take,
            Authors = authorsResponse
        };

        return topAuthorsResponse;
    }

    public async ValueTask<GetTopTagsResponse> GetTopTagsAsync(GetTopTagsRequest paginationRequest)
    {
        const string BASE_QUERY = $"SELECT * FROM c WHERE c.type = \"{nameof(Tag)}\" ";
        StringBuilder queryBuilder = new(BASE_QUERY);

        if (!paginationRequest.Text.IsNullOrEmpty())
        {
            queryBuilder.Append($"AND CONTAINS(LOWER(c.{nameof(Tag.Name).FirstLetterToLower()}), LOWER(\"{paginationRequest.Text.Trim()}\"))");
        }

        var tagsList = await _repositoryTag.GetByQueryAsync(queryBuilder.ToString());

        if (tagsList == null)
            throw new NotFoundException(ErrorMessages.TAG_NOT_EXISTS);

        var tagsWithArticles = tagsList.Where(c => c.ArticleIds.Count > 0).OrderByDescending(c => c.ArticleIds.Count);

        var totalItems = tagsWithArticles.Count();

        if (totalItems <= paginationRequest.Skip)
            throw new NotFoundException(ErrorMessages.TAG_NOT_EXISTS);

        if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
            paginationRequest.Take = (totalItems - paginationRequest.Skip);

        var tagsResponse = tagsWithArticles
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .OrderByDescending(c => c.ArticleIds.Count)
            .Select(tag => _mapper.Map<TagResponse>(tag))
            .ToList();

        var topTagsResponse = new GetTopTagsResponse()
        {
            TotalItems = totalItems,
            Skip = paginationRequest.Skip,
            Take = paginationRequest.Take,
            Tags = tagsResponse
        };

        return topTagsResponse;
    }

    private async ValueTask<List<Tag>> AllocateTags(List<string> tagsList, Article articleEntity)
    {
        const int TAGS_MAX_QUANTITY = 5;
        const string TAGS_PATTERN = @"#[A-Za-z0-9]+";

        List<Tag> tagsForArticleEntityList = new();

        if (tagsList.IsNullOrEmpty())
        {
            return tagsForArticleEntityList;
        }

        if (tagsList.Count > TAGS_MAX_QUANTITY)
            throw new ValidationException(ErrorMessages.ARTICLE_MAX_TAGS_QUANTITY);

        List<string> matchingTags = new();

        foreach (var tag in tagsList)
        {
            MatchCollection matchedTagsFromList = Regex.Matches(tag, TAGS_PATTERN);

            foreach (Match match in matchedTagsFromList)
            {
                matchingTags.Add(match.Value);
            }
        }

        foreach (var tag in matchingTags)
        {
            var existingEntityTag = await _repositoryTag.GetAsync(c => c.Name == tag).FirstOrDefaultAsync();

            if (existingEntityTag == null)
            {
                var newTag = new Tag
                {
                    Name = tag,
                    ArticleIds = new List<string> { articleEntity.Id },
                };
                await _repositoryTag.CreateAsync(newTag);

                articleEntity.TagIds.Add(newTag.Id);
                tagsForArticleEntityList.Add(newTag);
            }
            else
            {
                articleEntity.TagIds.Add(existingEntityTag.Id);
                tagsForArticleEntityList.Add(existingEntityTag);

                if (!existingEntityTag.ArticleIds.Contains(articleEntity.Id))
                {
                    existingEntityTag.ArticleIds.Add(articleEntity.Id);
                    await _repositoryTag.UpdateAsync(existingEntityTag);
                }
            }
        }

        return tagsForArticleEntityList;
    }

    private async ValueTask<string> BuildQuery(GetAllArticlesRequest paginationRequest, string userWhoLikedPages = "")
    {
        List<Tag> tagsList = new();

        const string baseQuery = $"SELECT * FROM c WHERE c.type = \"{nameof(Article)}\" ";
        StringBuilder queryBuilder = new(baseQuery);


        if (!userWhoLikedPages.IsNullOrEmpty())
        {
            queryBuilder.Append($"AND ARRAY_CONTAINS(c.{nameof(Article.LikedUserIds).FirstLetterToLower()}, \"{userWhoLikedPages}\") ");
        }

        if (!paginationRequest.Tags.IsNullOrEmpty())
        {
            foreach (var tag in paginationRequest.Tags)
            {
                if (tag == null)
                    throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

                var tagEntity = await _repositoryTag.GetAsync(c => c.Name.ToLower() == tag.ToLower()).FirstOrDefaultAsync();

                if (tagEntity == null)
                    throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

                queryBuilder.Append($"AND ARRAY_CONTAINS(c.{nameof(Article.TagIds).FirstLetterToLower()}, \"{tagEntity.Id}\") ");
            }
        }

        if (!paginationRequest.Text.IsNullOrEmpty())
        {
            List<string> wordList = paginationRequest.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (var word in wordList)
            {
                queryBuilder.Append("AND (");
                queryBuilder.Append(
                    $"CONTAINS(LOWER(c.{nameof(Article.Name).FirstLetterToLower()}), LOWER(\"{word}\")) " +
                    $"OR CONTAINS(LOWER(c.{nameof(Article.UserName).FirstLetterToLower()}), LOWER(\"{word}\"))  " +
                    $"OR CONTAINS(LOWER(c.{nameof(Article.Text).FirstLetterToLower()}), LOWER(\"{word}\"))");
                queryBuilder.Append(") ");
            }
        }

        queryBuilder.Append(" ORDER BY c.");

        if (paginationRequest.SortBy.ToString() == nameof(SortOrder.Ascending))
        {
            queryBuilder.Append("createdTimeUtc");
        }
        else if (paginationRequest.SortBy.ToString() == nameof(SortOrder.TopRated))
        {
            queryBuilder.Append(nameof(Article.LikesCount).FirstLetterToLower());
            queryBuilder.Append(" DESC");
        }
        else
        {
            queryBuilder.Append("createdTimeUtc");
            queryBuilder.Append(" DESC");
        }

        return queryBuilder.ToString();
    }

    private async ValueTask<GetArticleResponse> ConvertGetArticleResponse(Article article)
    {
        var userId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;
        bool IsLikedByUser = false;
        if (article.LikedUserIds.Contains(userId))
        {
            IsLikedByUser = true;
        }

        string queryTagsByArticleId = $"SELECT * FROM c WHERE c.type = \"{nameof(Tag)}\" AND ARRAY_CONTAINS(c.{nameof(Tag.ArticleIds).FirstLetterToLower()}, '{article.Id}')";
        var tagsList = await _repositoryTag.GetByQueryAsync(queryTagsByArticleId);

        string queryCommentsQuantity = $"SELECT * FROM c WHERE c.type = \"{nameof(Comment)}\" AND c.{nameof(Comment.ArticleId).FirstLetterToLower()} = \"{article.Id}\"";
        var commentsList = await _repositoryComment.GetByQueryAsync(queryCommentsQuantity);

        var imageResponse = await _imageService.GetAsync(article.ImageFileName);

        GetArticleResponse articleResponse = _mapper.Map<GetArticleResponse>(article);
        articleResponse.Image = imageResponse;
        articleResponse.Tags = tagsList.Select(tag => _mapper.Map<TagResponse>(tag)).ToList();
        articleResponse.CommentsCount = commentsList.Count();
        articleResponse.IsLikedByUser = IsLikedByUser;

        return articleResponse;
    }
}