using Microsoft.EntityFrameworkCore;
using OnlineMobileStore.Data;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace OnlineMobileStore.Models
{
 
    public class Order
    {


		private readonly ApplicationDbContext _context;

		public Order(ApplicationDbContext order)
		{
			_context = order;
		}
        public Order()
        {
       
        }
        public int Id { get; set; }
       
        public DateTime? OrderDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal? TotalPrice { get; set; }
        public int? TotalItems { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Payment Payment { get; set; }
        public List<OrderItem>? OrderItems { get; set; }





    }
}