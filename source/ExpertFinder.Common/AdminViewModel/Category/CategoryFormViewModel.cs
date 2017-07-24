using System;
using System.ComponentModel.DataAnnotations;

namespace ExpertFinder.Common.AdminViewModel
{
    public class CategoryFormViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Kategori adını giriniz.")]
        [Display(Name = "Kategori Adı")]
        public string Name { get; set; }

        public Guid? ParentId { get; set; }

        public int? Priority { get; set; }

        public string Icon { get; set; }

        public string ImageUrl { get; set; }

        public bool IsUrun { get; set; }

        public string SlugUrl { get; set; }

        public string UIUrl { get; set; }
    }
}