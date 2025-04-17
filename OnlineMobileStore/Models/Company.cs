using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineMobileStore.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IFormFile? ProfileImageFile { get; set; }
        public List<Mobile>? Mobiles { get; set; } = new();
	}
}