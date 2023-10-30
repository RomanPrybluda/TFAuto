namespace TFAuto.Domain.Services
{
    public class BasePaginationResponse : BasePaginationRequest
    {
        public int TotalItems { get; set; }
    }
}