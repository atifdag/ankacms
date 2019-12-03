using AnkaCMS.Core.ValueObjects;
using System.Collections.Generic;

namespace AnkaCMS.Core
{
    public interface ISmtp
    {
        void Send(EmailMessage eMailMessage);
        void SendWithTemplate(EmailMessage emailMessage, string emailTemplate, List<EmailRow> emailRows);
    }
}