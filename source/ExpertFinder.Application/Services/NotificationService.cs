using Common.Entities;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.Helper;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpertFinder.Domain
{
    public class NotificationService : INotificationService
    {
        private readonly teklifcepteDBContext _dbContext;
        private readonly IConfiguration _configuration;

        private string FrontendUrl
        {
            get
            {
                var url = _configuration["Frontend:Domain"];
                return url.Substring(0, url.Length - 1);
            }
        }

        public NotificationService(
            teklifcepteDBContext dbContext,
            IConfiguration configuration
            )
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public IEnumerable<Notifications> GetNotifications(string userId)
        {
            var notifications = _dbContext.Notifications
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.AddedDate)
                .ToList();

            return notifications;
        }

        public Notifications GetNotification(Guid Id)
        {
            var notification = _dbContext.Notifications
                .FirstOrDefault(i => i.Id == Id);

            return notification;
        }

        private User GetUser(string userId)
        {
            var user = _dbContext.Users
                  .FirstOrDefault(i => i.Id == userId);

            return user;
        }

        private Item GetItem(Guid itemId)
        {
            var item = _dbContext.Item
                .Include(i => i.Category)
                .FirstOrDefault(i => i.Id == itemId);

            return item;
        }

        public void AddNotification(string userId, NotificationTypes type, string param = null, string itemID = null)
        {
            var user = GetUser(userId);

            if (user != null)
            {
                var description = string.Empty;

                var mailTemplateId = string.Empty;
                var mailSubject = string.Empty;
                var mailReplacements = new Dictionary<string, string>();
                var smsMessage = string.Empty;

                try
                {
                    switch (type)
                    {
                        case NotificationTypes.Registration:
                            description = "Üyeliğiniz gerçekleşti.";
                            mailTemplateId = "8cbf7cb4-59f0-4d4b-93c0-1de316d57e32";
                            break;

                        case NotificationTypes.RegistrationExternalProvider:
                            description = string.Format("{0} ile üyelik gerçekleştirdiniz.", param);
                            mailTemplateId = "8cbf7cb4-59f0-4d4b-93c0-1de316d57e32";
                            break;

                        case NotificationTypes.ForgetPassMail:
                            description = "Şifre sıfırlama mailini yollandız.";

                            mailSubject = "Şifre Yenileme";
                            mailTemplateId = "af4d7a73-5ae9-4eff-9240-e386ed3be04d";
                            mailReplacements.Add("url", FrontendUrl + param);

                            break;

                        case NotificationTypes.CellPhoneVerified:
                            description = "Cep telefonunuzu onaylandınız.";
                            break;

                        case NotificationTypes.ProfileSaved:
                            description = "Profilinizde değişiklik yaptınız.";
                            break;

                        case NotificationTypes.LinkExternalProvider:
                            description = string.Format("{0} ile hesabınız doğruladınız.", param);
                            break;

                        case NotificationTypes.UnLinkExternalProvider:
                            description = string.Format("Hesabınızdan {0} bağlantısını kaldırdınız.", param);
                            break;

                        case NotificationTypes.ChangedPassword:
                            description = "Şifrenizi değiştirdiniz.";

                            if (!user.SmsNotAllowed.HasValue || !user.SmsNotAllowed.Value)
                                smsMessage = "Şifrenizi değiştirdiniz.";
                            break;

                        case NotificationTypes.SetPassword:
                            description = "Hesabınıza giriş için şifre oluşturdunuz.";
                            break;

                        case NotificationTypes.EmailVerified:
                            description = "Email adresinizi onaylandınız.";
                            break;

                        case NotificationTypes.CreateItem:
                            description = "Yeni bir hizmet verdiniz.";
                            break;

                        case NotificationTypes.EditItem:
                            description = "Hizmetinizi güncellediniz.";
                            break;

                        case NotificationTypes.CreateOffer:
                            description = "Teklif verdiniz.";
                            break;

                        case NotificationTypes.ReceivedOffer:
                            description = "Tebrikler, hizmetinize teklif aldınız.";

                            if (!user.SmsNotAllowed.HasValue || !user.SmsNotAllowed.Value)
                                if (!string.IsNullOrWhiteSpace(itemID))
                                {
                                    var item = GetItem(new Guid(itemID));
                                    if (item != null)
                                    {
                                        var url = string.Format("{0}/{1}/{2}", FrontendUrl, item.Category.SlugUrl, item.SlugUrl);

                                        smsMessage = string.Format("Hizmetinize teklif aldınız. {0}", url);
                                    }
                                }
                            break;

                        case NotificationTypes.AcceptOffer:
                            description = "Teklifi kabul ettiniz.";
                            break;

                        case NotificationTypes.ReceivedAcceptOffer:
                            description = "Tebrikler, teklifiniz kabul edildi.";

                            if (!user.SmsNotAllowed.HasValue || !user.SmsNotAllowed.Value)
                                if (!string.IsNullOrWhiteSpace(itemID))
                                {
                                    var item = GetItem(new Guid(itemID));
                                    if (item != null)
                                    {
                                        var url = string.Format("{0}/{1}/{2}", FrontendUrl, item.Category.SlugUrl, item.SlugUrl);

                                        smsMessage = string.Format("Teklifiniz kabul edildi. {0}", url);
                                    }
                                }
                            break;

                        case NotificationTypes.PaymentComplete:
                            description = "Ödeme yaptınız!";

                            if (!user.SmsNotAllowed.HasValue || !user.SmsNotAllowed.Value)
                                smsMessage = string.Format("{0} TL tutarında ödeme gerçekleştirdiniz.", param);
                            break;

                        case NotificationTypes.UpdateAdmin:
                            description = "Admin kullanıcısı güncellediniz.";
                            break;

                        case NotificationTypes.AdminInfoMail:
                            mailSubject = "Admin Kullanıcı Bilgileri";
                            mailTemplateId = "bcbd1edc-6820-4225-9b0a-1004d5673f7a";
                            mailReplacements.Add("password", param);
                            break;

                        case NotificationTypes.SendMailVerify:
                            mailSubject = "Email Onayla";
                            mailTemplateId = "4bfe4702-f737-457c-9234-8e68cd11ce78";
                            mailReplacements.Add("url", FrontendUrl + param);
                            break;

                        case NotificationTypes.AdminToken:
                            description = "Hesabınıza Teklif Cepte tarafından jeton yüklendi.";
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(description))
                    {
                        var notification = new Notifications()
                        {
                            Id = Guid.NewGuid(),
                            AddedDate = DateTime.Now,
                            Description = description,
                            NotificationTypeId = (int)type,
                            UserId = user.Id
                        };

                        if (!string.IsNullOrWhiteSpace(itemID))
                            notification.ItemID = new Guid(itemID);

                        _dbContext.Notifications.Add(notification);

                        _dbContext.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(mailTemplateId))
                        SendEmail(user, mailSubject, mailTemplateId, mailReplacements);

                    if (!string.IsNullOrEmpty(smsMessage))
                        SendSMS(user, smsMessage);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void AddReplacement(Dictionary<string, string> replacements, string key, string value)
        {
            if (!replacements.ContainsKey(key))
                replacements.Add(key, value);
        }

        private TransactionResult SendEmail(User user, string subject, string templateId, Dictionary<string, string> mailReplacements)
        {
            var replacements = new Dictionary<string, string>(mailReplacements);
            var email = user.Email;

            AddReplacement(replacements, "email", user.Email);

            //TODO Uncomment here
            return null; // EmailHelper.SendMail(email, subject, " ", templateId, replacements);
        }

        private string SendSMS(User user, string smsMessage)
        {
            var phoneNumber = user.PhoneNumber;
            return SmsHelper.SendSMS(phoneNumber, smsMessage);
        }

        public string SendSMSVerificationNotification(VerificationTypes type, string phone, string verificationCode)
        {
            string smsBody = string.Empty;
            switch (type)
            {
                case VerificationTypes.Registration:
                    smsBody = string.Format("Üyeliğinizi aktive etmek için gerekli onay kodunuz {0} olup 3 dakika için geçerlidir.", verificationCode);
                    break;

                case VerificationTypes.PasswordRenewal:
                    smsBody = string.Format("Şifrenizi değiştirmek için gerekli onay kodunuz {0} olup 3 dakika için geçerlidir.", verificationCode);
                    break;
            }

            return SmsHelper.SendSMS(phone, smsBody);
        }
    }
}