using Microsoft.AspNetCore.Identity;

namespace Eticaret.Models
{
	//kullanıcıya rol atamak için kullandık.
	public class UserRol
	{	
        public int Id { get; set; }
        public string UserId { get; set; } //hem metinsel hem sayısal değerler oldugu için string yaptık.    
        public int RolId { get; set; }
	
	}
}
