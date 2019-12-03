using AnkaCMS.Core.Security;
using System;

namespace AnkaCMS.Service
{
    public interface IIdentityService
    {
        void Set(AnkaCMSIdentity identity, DateTime expires, bool rememberMe);
        AnkaCMSIdentity Get();
        void Remove();
    }
}
