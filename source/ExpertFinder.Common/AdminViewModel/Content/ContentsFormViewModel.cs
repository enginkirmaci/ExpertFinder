using System;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.AdminViewModel
{
    public class ContentsFormViewModel
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        [Display(Name = "Sayfa Başlık")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Sayfa içeriğini giriniz.")]
        [Display(Name = "Sayfa İçerik")]
        public string Value { get; set; }
    }
}