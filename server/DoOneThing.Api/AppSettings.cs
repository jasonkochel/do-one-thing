using System.Diagnostics.CodeAnalysis;

namespace DoOneThing.Api
{
    public class AppSettings
    {
        public GoogleCredentials GoogleCredentials { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class GoogleCredentials
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
    }
}