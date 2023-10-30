using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Blob.DTO
{
    public class GetFileRequest
    {
        [Required]
        public string FileName { get; set; }
    }
}