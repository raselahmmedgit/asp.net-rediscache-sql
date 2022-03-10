namespace lab.RedisCacheSql.Config
{
    public class AppSmsConfig
    {
        public static string Name = "AppSmsConfig";
        public string FromNumber { get; set; }
        public string TwilioFromNumber { get; set; }
        public string TwilioAccountSid { get; set; }
        public string TwilioAuthToken { get; set; }
    }
}
