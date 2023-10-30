namespace TFAuto.Domain.Services.Admin.DTO.Request
{
    public class GetUsersPaginationRequest : BasePaginationRequest
    {
        public string Text { get; set; }

        public SortOrderUsers SortBy { get; set; } = SortOrderUsers.UserNameAscending;
    }
}