using ExpertFinder.Models;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.ViewModels.Manage
{
    public class SettingsViewModel
    {
        public User User { get; set; }

        public bool HasPassword { get; set; }

        public IList<UserLoginInfo> CurrentLogins { get; set; }

        public IList<AuthenticationDescription> OtherLogins { get; set; }

        [Required(ErrorMessage = "Eski şifrenizi girmelisiniz.")]
        [DataType(DataType.Password)]
        [Display(Name = "Eski Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifrenizi girmelisiniz.")]
        [StringLength(100, ErrorMessage = "Şifreniz en az 6 haneli olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Onay")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifre ve yeni şifre onayı aynı olmalıdır.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Sms ile mesaj almak istemiyorum.")]
        public bool SmsNotAllowed { get; set; }
    }
}