namespace mvcClient.Configuration
{
    public class IdentityServerSettings
    {
        public string DiscoveryURL { get; set; }
        public string ClientName { get; set; }
        public string ClientPassword { get; set; }
        public bool UseHttps { get; set; }
    }
}
