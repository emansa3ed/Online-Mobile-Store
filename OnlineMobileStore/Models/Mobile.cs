using System.ComponentModel.DataAnnotations;

namespace OnlineMobileStore.Models
{
    public class Mobile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Price { get; set; }
        public string Processor { get; set; }
        public string Screen { get; set; }
        public string Battery { get; set; }
        public string OS { get; set; }
        public string Camera { get; set; }
        public string Image { get; set; }    
        public int CompanyId { get; set; }
        public Company Company { get; set; }
		public List<OrderItem> ? OrderItems { get; set; }
	}
}
