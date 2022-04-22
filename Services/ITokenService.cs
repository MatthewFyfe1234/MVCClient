using IdentityModel.Client;

namespace mvcClient.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
}
