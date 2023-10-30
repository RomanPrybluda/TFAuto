namespace TFAuto.DAL.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; }

        public List<string> PermissionIds { get; set; }

        public override string PartitionKey { get; set; } = nameof(Role);
    }
}
