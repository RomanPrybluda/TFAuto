namespace TFAuto.DAL.Constant
{
    public class PermissionId
    {
        public const string READ_ARTICLES = "c591984b-ccbc-484b-90db-01e390ffd131";
        public const string EDIT_ARTICLES = "c591984b-ccbc-484b-90db-01e390ffd132";
        public const string MANAGE_ARTICLES = "c591984b-ccbc-484b-90db-01e390ffd133";
        public const string MANAGE_ROLES = "c591984b-ccbc-484b-90db-01e390ffd134";
        public const string DELETE_COMMENT = "c591984b-ccbc-484b-90db-01e390ffd135";
        public const string MANAGE_USERS = "c591984b-ccbc-484b-90db-01e390ffd136";
    }

    public static class PermissionIdList
    {
        static readonly List<string> _permissionList = new();

        static PermissionIdList()
        {
            _permissionList.Add(PermissionId.READ_ARTICLES);
            _permissionList.Add(PermissionId.EDIT_ARTICLES);
            _permissionList.Add(PermissionId.MANAGE_ARTICLES);
            _permissionList.Add(PermissionId.MANAGE_ROLES);
            _permissionList.Add(PermissionId.DELETE_COMMENT);
            _permissionList.Add(PermissionId.MANAGE_USERS);
        }

        public static List<string> GetPermissions()
        {
            return _permissionList;
        }
    }
}
