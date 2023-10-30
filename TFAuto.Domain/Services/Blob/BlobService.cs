using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Services.Blob.DTO;

namespace TFAuto.Domain.Services.Blob
{
    public class BlobService : IBlobService
    {
        private readonly BlobContainerClient _container;
        private readonly IConfiguration _configuration;

        public BlobService(IConfiguration configuration)
        {
            _configuration = configuration;
            var blobStorageConnectionString = configuration.GetConnectionString("BlobStorageConnectionString");
            var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
            var blobStorageSettings = GetBlobStorageSettings();
            _container = blobServiceClient.GetBlobContainerClient(blobStorageSettings.ContainerName);
        }

        public async ValueTask<GetFileResponse> GetAsync(string fileName)
        {
            if (fileName == null)
                throw new NotFoundException(ErrorMessages.FILE_NOT_FOUND);

            BlobClient blob = _container.GetBlobClient(fileName);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new NotFoundException(ErrorMessages.FILE_NOT_FOUND);

            string uri = _container.Uri.ToString();
            var name = blob.Name;
            var fullUri = $"{uri}/{name}";

            var content = await blob.DownloadContentAsync();
            string contentType = content.Value.Details.ContentType;

            GetFileResponse response = new()
            {
                Uri = fullUri,
                FileName = name,
                ContentType = contentType
            };

            return response;
        }

        public async ValueTask<UploadFileResponse> UploadAsync(IFormFile uploadFile)
        {
            if (uploadFile == null)
                throw new ValidationException(ErrorMessages.FILE_OR_REQUEST_INVALID);

            string[] allowedExtensions = GetBlobStorageSettings().AllowedFileExtensions;
            var fileExtension = Path.GetExtension(uploadFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ValidationException(ErrorMessages.FILE_ALLOWED_EXTENSIONS + string.Join(" , ", allowedExtensions));
            }

            var blobStorageSettings = GetBlobStorageSettings();

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(uploadFile.FileName);
            string storageFileName = $"{fileNameWithoutExtension}-{Guid.NewGuid()}";

            BlobClient blob = _container.GetBlobClient(storageFileName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                await uploadFile.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var blobUploadOptions = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = blobStorageSettings.ContentType
                    }
                };

                await blob.UploadAsync(memoryStream, blobUploadOptions);
            }

            UploadFileResponse fileResponse = new()
            {
                Message = $"File {uploadFile.FileName} uploaded Successfully",
                Uri = blob.Uri.AbsoluteUri,
                FileName = blob.Name
            };

            return fileResponse;
        }

        public async ValueTask<UploadFileResponse> UpdateAsync(string existingFileNameRequest, IFormFile updatedFile)
        {
            BlobClient blob = _container.GetBlobClient(existingFileNameRequest);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new NotFoundException(ErrorMessages.FILE_NOT_FOUND);

            UploadFileResponse fileResponse = new()
            {
                Uri = blob.Uri.AbsoluteUri,
                FileName = blob.Name,
                Message = $"New file wasn't uploaded",
            };

            if (updatedFile != null)
            {
                fileResponse = await UploadAsync(updatedFile);
                await blob.DeleteAsync();
            }

            return fileResponse;
        }

        public async ValueTask<DownloadFileResponse> DownloadAsync(string fileName)
        {
            BlobClient blob = _container.GetBlobClient(fileName);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new NotFoundException(ErrorMessages.FILE_NOT_FOUND);

            var blobStorageSettings = GetBlobStorageSettings();

            var content = await blob.DownloadAsync();
            Stream fileContent = content.Value.Content;

            string name = $"{fileName}{blobStorageSettings.DowloadFileExtensions}";
            string contentType = content.Value.Details.ContentType;

            var message = new DownloadFileResponse
            {
                Content = fileContent,
                FileName = name,
                ContentType = contentType
            };

            return message;
        }

        public async ValueTask<DeleteFileResponse> DeleteAsync(string fileName)
        {
            BlobClient blob = _container.GetBlobClient(fileName);

            bool exists = await blob.ExistsAsync();

            if (!exists)
                throw new NotFoundException(ErrorMessages.FILE_NOT_FOUND);

            await blob.DeleteAsync();

            var message = new DeleteFileResponse { Success = true, Message = $"File: {fileName} has been successfully deleted." };
            return message;
        }

        private BlobStorageSettings GetBlobStorageSettings()
        {
            return _configuration.GetSection("BlobStorageSettings").Get<BlobStorageSettings>();
        }
    }
}