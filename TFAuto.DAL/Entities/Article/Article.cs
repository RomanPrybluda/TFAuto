namespace TFAuto.DAL.Entities.Article;

public class Article : BaseEntity
{
    public string Name { get; set; }

    public string Text { get; set; }

    public string UserId { get; set; }

    public string UserName { get; set; }

    public string LastUserWhoUpdated { get; set; }

    public List<string> TagIds { get; set; } = new();

    public List<string> LikedUserIds { get; set; } = new();

    public int LikesCount { get; set; }

    public string ImageFileName { get; set; }

    public override string PartitionKey { get; set; } = nameof(Article);
}
