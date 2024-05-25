using Eticaret.Data;
using Eticaret.Helper;
using Eticaret.Models;
using Eticaret.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;

namespace Eticaret.Controllers
{
	public class ShoppingCartController : Controller
	{

		//contexti oluşturalım.

		public EticaretVeritabaniContext _context { get; set; }
		public ApplicationDbContext _Appcontext { get; set; }

		private ShoppingCart shoppingCart;

        public ShoppingCartController(EticaretVeritabaniContext eticaretVeritabaniContext,ApplicationDbContext applicationDbContext)
        {
			_context = eticaretVeritabaniContext;
			shoppingCart = new ShoppingCart();
			shoppingCart._context = eticaretVeritabaniContext;
			_Appcontext = applicationDbContext;
        }



		//AddToCart: sepete ürün eklemek istenince çalıştırılacak metot.
		public IActionResult AddToCart(int AlbumId, int adet)
		{
			var addedAlbum=_context.Albums.Single(satir => satir.AlbumId == AlbumId);
			var cart=shoppingCart.GetCartItems(this.HttpContext); //eski eklenenleri getirecek metot.
            for (int i = 0; i < adet; i++)
            {
				cart.AddToCart(addedAlbum);
				HttpContext.Session.SetString("adet", cart.GetCount().ToString()); //session değerini sepetteki adet bilgisini güncelledik. 
			}
			
			return RedirectToAction("Index");
		}

        public IActionResult Index()
		{
			//ödeme öncesi ekran.
			//o anki sepette olan ürünleri liste halinde sayfada gösterme

			var cart=shoppingCart.GetCart(this.HttpContext);
			HttpContext.Session.SetString("adet",cart.GetCount().ToString());

			var viewModel = new ShoppingCartViewModel
			{
				CartItems = cart.GetCartItems(),
				CartTotal = cart.GetTotal()
			};

			HelperClass helperClass = new HelperClass(_Appcontext, _context);
			string AlbumIds=helperClass.GetAlbumIds(cart.GetCartItems());

			HttpContext.Session.SetString("alisverisTutari", cart.GetTotal().ToString());
			HttpContext.Session.SetString("albumIds", AlbumIds);
		
			ViewData["SepetAdet"] = HttpContext.Session.GetString("adet");
			return View(viewModel);
		}

		//ajax post işlemi
		
		public IActionResult RemoveFromCart(int id)
		{
			var albumName=_context.Carts.Include(satir => satir.Album).Single(item => item.RecordId == id).Album.Baslik;
			var cart=shoppingCart.GetCart(this.HttpContext);

			int itemCount=cart.RemoveFromCart(id);

			HttpContext.Session.SetString("adet", cart.GetCount().ToString());


			var results = new ShoppingRemoveViewModel
			{
				Mesage = HtmlEncoder.Default.Encode(albumName) + " silinecek",
				CartTotal = cart.GetTotal(),
				CartCount = cart.GetCount(),
				ItemCount = itemCount,
				DeleteId = id
			};


			return Json(results);
		}


		//emptyCart metodu
		public IActionResult EmptyCart()
		{
			var cart = shoppingCart.GetCart(this.HttpContext);

			var cartItems = _context.Carts.Where(satir => satir.CartId == cart.ShoppingCartId);
			foreach (var item in cartItems)
			{
				_context.Carts.Remove(item);
			}
			_context.SaveChanges();

			HttpContext.Session.SetString("adet","0"); //sepetteki herşeyi sildik , sepeti 0'ladık
			return RedirectToAction("Index","Home");
		}

	}
}
