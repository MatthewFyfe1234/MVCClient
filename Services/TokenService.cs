using IdentityModel.Client;
using Microsoft.Extensions.Options;
using mvcClient.Configuration;

namespace mvcClient.Services
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocumentResponse;
        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> identityServerSettings)
        {
            _logger = logger;
            _identityServerSettings = identityServerSettings;

            using (var client = new HttpClient())
                _discoveryDocumentResponse = client.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryURL).Result;

            if (_discoveryDocumentResponse.IsError)
            {
                logger.LogError($"Unable to get discovery document {_discoveryDocumentResponse.Error}");
                throw new Exception($"Unable to get discovery document {_discoveryDocumentResponse.Error}");
            }
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            TokenResponse tokenResponse = new TokenResponse();
            using (var client = new HttpClient())
            {
                tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest{ 
                    Address = _discoveryDocumentResponse.TokenEndpoint,
                    ClientId = _identityServerSettings.Value.ClientName,
                    ClientSecret = _identityServerSettings.Value.ClientPassword,
                    Scope = scope
                });
            }
            if (tokenResponse.IsError)
            {
                _logger.LogError($"Unable to get token {tokenResponse.Error}");
                throw new Exception($"Unable to get token {tokenResponse.Error}");
            }
            else
                return tokenResponse;
        }
    }
}
