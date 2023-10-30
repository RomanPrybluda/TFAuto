using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.Blob.DTO
{
    public class UploadFileRequest
    {
        [Required]
        public IFormFile File { get; set; }
    }
}