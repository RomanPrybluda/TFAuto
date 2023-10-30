namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse : BasePaginationResponse
{
    public List<GetArticleResponse> Articles { get; set; } = new();

    public SortOrder OrderBy { get; set; }
}