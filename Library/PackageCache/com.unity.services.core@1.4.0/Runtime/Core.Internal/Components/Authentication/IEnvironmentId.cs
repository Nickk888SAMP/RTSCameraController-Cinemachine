using Unity.Services.Core.Internal;

namespace Unity.Services.Authentication.Internal
{
    /// <summary>
    /// Component providing the Environment Id
    /// </summary>
    public interface IEnvironmentId : IServiceComponent
    {
        /// <summary>
        /// Returns the Environment ID when a sign in succeeds, otherwise null.
        /// </summary>
        string EnvironmentId { get; }
    }
}
