using System;
using System.Threading.Tasks;

namespace Unity.Services.Vivox.Internal
{
    /// <summary>
    /// Must be implemented by the <see cref="IVivox.RegisterTokenProvider(IVivoxTokenProviderInternal)"/> caller.
    /// This object's responsibility is to provide an overridable implementation that will generate tokens for Vivox actions.
    /// </summary>
    public interface IVivoxTokenProviderInternal
    {
        /// <summary>
        /// This async method should implement the necesary steps to providing a valid Vivox token.
        /// After registration, this method will automatically be called whenever a token needs to be generated for a particular action. (e.g. login, channel join, mute)
        /// This token generation method will not be used if a developer provides their own IVivoxTokenProvider imeplemntation to the Vivox service.
        /// If the requested action is a login to the Vivox service we will use the UAS token for that instead of this method as well.
        /// </summary>
        /// <returns>A Vivox token string.</returns>
        Task<string> GetTokenAsync(string issuer = null, TimeSpan? expiration = null, string userUri = null, string action = null, string conferenceUri = null, string fromUserUri = null, string realm = null);
    }
}
