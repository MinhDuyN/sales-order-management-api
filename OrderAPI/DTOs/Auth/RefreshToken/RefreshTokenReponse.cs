namespace OrderAPI.DTOs.Auth.RefreshToken
{
    public class RefreshTokenReponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
