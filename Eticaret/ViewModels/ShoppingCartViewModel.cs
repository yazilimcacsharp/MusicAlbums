using Eticaret.Models;

namespace Eticaret.ViewModels
{
	public class ShoppingCartViewModel
	{
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
