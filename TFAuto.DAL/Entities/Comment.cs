namespace TFAuto.DAL.Entities
{
    public class Comment : BaseEntity
    {
        public string ArticleId { get; set; }

        public string AuthorId { get; set; }

        public string Content { get; set; }

        public int LikesCount { get; set; }

        public override string PartitionKey { get; set; } = nameof(Comment);
    }
}