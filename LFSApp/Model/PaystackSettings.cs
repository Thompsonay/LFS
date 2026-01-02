namespace LFSApp.Model
{
    public class PaystackSettings
    {
        public string BaseUrl { get; set; } = "https://api.paystack.co";
        public string PublicKey { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }
}
