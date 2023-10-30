using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Blob.DTO
{
    public class DeleteFileRequest
    {
        [Required]
        public string FileName { get; set; }
    }
}