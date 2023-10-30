namespace TFAuto.DAL.Entities
{
    public class Like : BaseEntity
    {
        public string CommentId { get; set; }

        public string UserId { get; set; }

        public override string PartitionKey { get; set; } = nameof(Like);
    }
}