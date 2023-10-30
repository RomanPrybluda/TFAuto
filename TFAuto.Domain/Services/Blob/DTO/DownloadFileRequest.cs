using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Blob.DTO
{
    public class DownloadFileRequest
    {
        [Required]
        public string FileName { get; set; }
    }
}