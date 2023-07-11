using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Networking;

namespace Unity.Services.Core.Editor
{
    public class AccessTokens
    {
        TokenExchange m_TokenExchange;

        internal AccessTokens(TokenExchange tokenExchange)
        {
            m_TokenExchange = tokenExchange;
        }

        public AccessTokens()
        {
            m_TokenExchange = new TokenExchange(new ProductionTokenExchangeUrls());
        }

        /// <summary>
        /// The access token used by Genesis
        /// </summary>
        /// <returns>Genesis Access Token</returns>
        public static string GetGenesisToken()
        {
            return CloudProjectSettings.accessToken;
        }

        /// <summary>
        /// Task that represents an asynchronous operation to get services gateway token.
        /// </summary>
        /// <returns>Task with a result that represents the services gateway token</returns>
        public async Task<string> GetServicesGatewayTokenAsync()
        {
            var genesisToken = GetGenesisToken();
            var token = await m_TokenExchange.ExchangeServicesGatewayTokenAsync(genesisToken);
            return token;
        }
    }
}
