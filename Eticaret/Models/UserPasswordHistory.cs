namespace Eticaret.Models
{
	public class UserPasswordHistory
	{
        //(Id, üretilensifre, uretilmeZamanı, kullanıcıMaili, aktifMi)

        public int UserPasswordHistoryId { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Mail { get; set; }
		public DateTime ExpireDate { get; set; } //skt: şifrenin girilebileceği son zmn bilgisi
		public bool IsActive { get; set; }
    }
}
