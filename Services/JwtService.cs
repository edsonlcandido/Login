namespace LoginApp.Services
{
    public class JwtService
    {
        private readonly string _secretKey;
        public JwtService(string secretKey)
        {
            _secretKey = secretKey;
        }
    }
}
