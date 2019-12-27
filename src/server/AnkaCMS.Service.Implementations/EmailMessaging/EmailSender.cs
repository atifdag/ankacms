using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AnkaCMS.Core;
using AnkaCMS.Core.Constants;
using AnkaCMS.Core.Enums;
using AnkaCMS.Core.Globalization;
using AnkaCMS.Core.Helpers;
using AnkaCMS.Core.ValueObjects;

namespace AnkaCMS.Service.Implementations.EmailMessaging
{
    public class EmailSender
    {
        private readonly ISmtp _smtp;
        private readonly IMainService _serviceMain;

        public EmailSender(ISmtp smtp, IMainService serviceMain)
        {
            _smtp = smtp;
            _serviceMain = serviceMain;
        }

        private static string ConvertTemplateToString(string emailTemplate, EmailRow emailRow)
        {
            return emailRow.EmailKeys.Aggregate(emailTemplate, (current, key) => current.TemplateParser(EmailConstants.EmailKeyRegEx.Replace(EmailConstants.EmailTokenName, key.Key.ToString()), key.Value));
        }
        public void SendEmailToUser(EmailUser user, EmailTypeOption emailTypes)
        {

            var projectRootPath = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));


            var templateFolderPath = Path.Combine(projectRootPath, _serviceMain.ApplicationSettings.EmailTemplatePath);
            var templateFilePath = Path.Combine(templateFolderPath, emailTypes + ".html");
            var templateText = FileHelper.ReadAllLines(templateFilePath);

            var from = new EmailAddress
            {
                DisplayName = _serviceMain.ApplicationSettings.SmtpSenderName,
                Address = _serviceMain.ApplicationSettings.SmtpSenderMail
            };

            string eMailSubject;

            var emailRow = new EmailRow();

            var emailKeys = new List<EmailKey>
            {
                new EmailKey
                {
                    Key = EmailKeyOption.ApplicationName,
                    Value = Dictionary.ApplicationName
                },
                new EmailKey
                {
                    Key = EmailKeyOption.ApplicationUrl,
                    Value = _serviceMain.ApplicationSettings.ApplicationUrl
                },
                new EmailKey
                {
                    Key = EmailKeyOption.FirstName,
                    Value = user.FirstName
                }
                ,
                new EmailKey
                {
                    Key = EmailKeyOption.LastName,
                    Value = user.LastName
                }
            };

            switch (emailTypes)
            {
                case EmailTypeOption.Add:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.SignUp:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.ActivationCode,
                            Value = (user.Id + "@" + user.CreationTime).Encrypt()
                        });
                        break;
                    }
                case EmailTypeOption.ForgotPassword:
                    {
                        eMailSubject = Dictionary.NewPassword;

                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.Update:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        break;
                    }

                case EmailTypeOption.UpdateMyPassword:
                    {
                        eMailSubject = Dictionary.NewPassword;
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Username,
                            Value = user.Username
                        });
                        emailKeys.Add(new EmailKey
                        {
                            Key = EmailKeyOption.Password,
                            Value = user.Password
                        });
                        break;
                    }
                case EmailTypeOption.UpdateMyInformation:
                    {
                        eMailSubject = Dictionary.UserInformation;
                        break;
                    }
                default:
                    {
                        throw new ArgumentOutOfRangeException(nameof(emailTypes), emailTypes, null);
                    }
            }

            emailKeys.Add(new EmailKey
            {
                Key = EmailKeyOption.Subject,
                Value = eMailSubject
            });

            emailRow.EmailKeys = emailKeys;

            var eMailMessage = new EmailMessage
            {
                To = new List<EmailAddress>
                {
                    new EmailAddress
                    {
                        Address = user.Email
                    }
                },
                Subject = eMailSubject,
                From = from,
                IsBodyHtml = true,
                Priority = EmailPriorityOption.Normal,
                Body = ConvertTemplateToString(templateText, emailRow)
            };

            _smtp.Send(eMailMessage);
        }
    }
}