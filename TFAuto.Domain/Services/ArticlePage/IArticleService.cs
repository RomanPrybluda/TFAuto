using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.Domain.Services.ArticlePage;

public interface IArticleService
{
    ValueTask<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest articleRequest);

    ValueTask<UpdateArticleResponse> UpdateArticleAsync(Guid articleId, UpdateArticleRequest articleRequest);

    ValueTask<GetSingleArticleResponse> GetArticleAsync(Guid articleId);

    ValueTask<GetAllArticlesResponse> GetAllArticlesAsync(GetAllArticlesRequest paginationRequest, string userWhoLikedPages = "");

    ValueTask<bool> SetLikeAsync(Guid articleId);

    ValueTask<bool> RemoveLikeAsync(Guid articleId);

    ValueTask<GetTopAuthorsResponse> GetTopAuthorsAsync(GetTopAuthorsRequest paginationRequest);

    ValueTask<GetTopTagsResponse> GetTopTagsAsync(GetTopTagsRequest paginationRequest);
}
