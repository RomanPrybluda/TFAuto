namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetTopAuthorsResponse : BasePaginationResponse
{
    public List<GetAuthorResponse> Authors { get; set; } = new();
}
