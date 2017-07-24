using ExpertFinder.Common.Enums;
using ExpertFinder.Models;
using System;
using System.Collections.Generic;

namespace ExpertFinder.Domain.Interfaces
{
    public interface INotificationService
    {
        IEnumerable<Notifications> GetNotifications(string userId);

        Notifications GetNotification(Guid Id);

        void AddNotification(string userId, NotificationTypes type, string param = null, string itemId = null);

        //void AddNotification(string userId, NotificationTypes type, string description = null, Guid? itemID = null);

        //TransactionResult SendForgetPassMail(string userId, string to, string phone, string link);

        //TransactionResult SendMailVerify(string to, string link);

        //TransactionResult SendAdminUserInfo(string userId, string to, string email, string password);

        string SendSMSVerificationNotification(VerificationTypes type, string phone, string verificationCode);
    }
}