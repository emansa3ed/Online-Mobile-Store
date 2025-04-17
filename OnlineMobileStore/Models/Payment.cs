namespace OnlineMobileStore.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } 
        public decimal Amount { get; set; }
        //Shipment Details
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public int OrderId {  get; set; }
		public Order? Order { get; set; }

	}
}
