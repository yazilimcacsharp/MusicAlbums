namespace Eticaret.Models
{
	public class UserPaymentCodeHistory
	{
        public int UserPaymentCodeHistoryId { get; set; }
        public string UserName { get; set; }
        public string Code  { get; set; }
        public DateTime SendDate { get; set; }
        public DateTime ExpireDate { get; set; } //2dk sonrsının bilgisini tutacak
        public bool IsPaymentReceived { get; set; } //ödeme alındı mı

    }
}
