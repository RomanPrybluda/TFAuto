namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetTopTagsResponse : BasePaginationResponse
{
    public List<TagResponse> Tags { get; set; } = new();
}
