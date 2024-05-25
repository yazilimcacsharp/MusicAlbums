using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Eticaret.Models
{
	public class IdentityUserRoleY: IdentityUserRole<string>
	{
		[ScaffoldColumn(false)]
        public string Id { get; set; }
		
	}
}
