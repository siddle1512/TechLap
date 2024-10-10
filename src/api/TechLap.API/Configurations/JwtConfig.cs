namespace TechLap.API.Configurations
{
    public static class JwtConfig
    {
        public static IConfiguration? _configuration { get; private set; }
        public static string secret { get; private set; } = string.Empty;

        public static void SetSecret(IConfiguration configuration)
        {
            _configuration = configuration;
            secret = configuration["Secret"] ?? throw new ArgumentNullException(nameof(configuration));
        }
    }
}
