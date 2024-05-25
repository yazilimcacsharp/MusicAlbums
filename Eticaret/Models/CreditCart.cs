namespace Eticaret.Models
{
	public class CreditCart
	{
        public int CreditCartId { get; set; }
        public string AdSoyad { get; set; }
		public string KartNumarasi { get; set; } //4444 4444 4444 4444
		public string SonKullanma { get; set; } //08/27 ...ay/yıl(2hane)
        public string CVV { get; set; } //arkadaki 3 hanel kod
        public decimal KullanılabilirLimit { get; set; }
        public bool AktifMi { get; set; }
    }
}
