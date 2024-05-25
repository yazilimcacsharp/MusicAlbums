using Microsoft.AspNetCore.Identity;

namespace Eticaret.ViewModels
{
    //kullanıcılar ve rollerini tek bir sayfada göstermek için class eklendi
	public class UserRole
	{
        public string Id { get; set; }
        public string UserName { get; set; }
        public string RolName { get; set; }
    }
}
