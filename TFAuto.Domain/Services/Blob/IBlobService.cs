using Microsoft.AspNetCore.Http;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public interface IBlobService
    {
        ValueTask<GetFileResponse> GetAsync(string fileName);

        ValueTask<UploadFileResponse> UploadAsync(IFormFile uploadedFile);

        ValueTask<DownloadFileResponse> DownloadAsync(string fileName);

        ValueTask<DeleteFileResponse> DeleteAsync(string fileName);

        ValueTask<UploadFileResponse> UpdateAsync(string oldFileName, IFormFile updateFile);
    }
}