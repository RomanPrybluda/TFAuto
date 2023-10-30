namespace TFAuto.Domain.Configurations
{
    public class BlobStorageSettings
    {
        public string ContainerName { get; set; }

        public string ContentType { get; set; }

        public string MaxAvatarImageFileSize { get; set; }

        public string MaxPostImageFileSize { get; set; }

        public string[] AllowedFileExtensions { get; set; }

        public string DowloadFileExtensions { get; set; }
    }
}