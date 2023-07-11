using System.Threading.Tasks;
using Unity.Services.Core.Internal;

namespace Unity.Services.Vivox.Internal
{
    /// <summary>
    /// Provides utilities for performing simple Vivox actions or overriding the <see cref="IVivoxTokenProviderInternal"/> with a custom implementation.
    /// </summary>
    public interface IVivox : IServiceComponent
    {
        /// <summary>
        /// Registers an <see cref="IVivoxTokenProviderInternal"/> that will be used as the primary token generator for all Vivox actions.
        /// </summary>
        void RegisterTokenProvider(IVivoxTokenProviderInternal tokenProvider);
    }
}
