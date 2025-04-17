using Microsoft.AspNetCore.Mvc.Rendering;
using OnlineMobileStore.Attributes;
using OnlineMobileStore.Settings;

using System.ComponentModel.DataAnnotations;

namespace OnlineMobileStore.ViewModels
{
    public class CreateMobileViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Display(Name="Company")]
        public int CompanyId { get; set; }
        public IEnumerable<SelectListItem> Companies { get; set; } = Enumerable.Empty<SelectListItem>();
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Processor { get; set; }
        public string Screen { get; set; }
        public string Battery { get; set; }
        public string OS { get; set; }
        public string Camera { get; set; }

        [AllowedExtensions(FileSettings.AllowedExtension)]
        public IFormFile? Image { get; set; } = default!;

    }
}
