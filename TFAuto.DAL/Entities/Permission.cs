namespace TFAuto.DAL.Entities
{
    public class Permission : BaseEntity
    {
        public string PermissionName { get; set; }

        public List<string> RoleIds { get; set; }

        public override string PartitionKey { get; set; } = nameof(Permission);
    }
}
