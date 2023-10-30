namespace TFAuto.Domain.Services;

public class GetAllArticlesRequest : BasePaginationRequest
{
    public string Text { get; set; }

    public List<string> Tags { get; set; } = new();

    public SortOrder SortBy { get; set; } = SortOrder.Descending;
}
