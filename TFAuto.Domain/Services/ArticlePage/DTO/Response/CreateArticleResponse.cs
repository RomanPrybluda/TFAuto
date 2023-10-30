using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class CreateArticleResponse
{
    public string Id { get; set; }

    public string Name { get; set; }

    public string Text { get; set; }

    public string UserName { get; set; }

    public string LastUserWhoUpdated { get; set; }

    public List<TagResponse> Tags { get; set; } = new();

    public UploadFileResponse Image { get; set; }

    public string CreatedTimeUtc { get; set; }

    public string LastUpdatedTimeUtc { get; set; }
}
