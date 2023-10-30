namespace TFAuto.Domain.Services.Blob.DTO
{
    public class DownloadFileResponse
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public Stream Content { get; set; }
    }
}