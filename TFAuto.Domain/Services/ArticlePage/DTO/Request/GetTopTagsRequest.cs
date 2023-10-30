namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class GetTopTagsRequest : BasePaginationRequest
{
    public string Text { get; set; }
}
