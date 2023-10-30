namespace TFAuto.Domain.Configurations
{
    public class SendGridSettings
    {
        public string ApiKey { get; set; }

        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string ContactUsEmail { get; set; }
    }
}
