using System.Security.Claims;

namespace AnkaCMS.Core.Security
{
    /// <inheritdoc />
    /// <summary>
    /// Yetkilendirilme işlemleri için temel sınıf
    /// </summary>
    public sealed class AnkaCMSPrincipal : ClaimsPrincipal
    {
        public AnkaCMSPrincipal(ClaimsIdentity identity)
        {
            AddIdentity(identity);
        }
    }
}
