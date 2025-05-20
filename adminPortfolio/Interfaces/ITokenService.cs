
namespace adminProfolio.Interfaces
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken)> GetTokensAsync(string userId, string email, string role);
    }
}