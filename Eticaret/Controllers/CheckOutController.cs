using Eticaret.Data;
using Eticaret.Helper;
using Eticaret.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eticaret.Controllers
{

	[Authorize] //login durumda bir kişi addressAndPayment sayfasına geçememeli.
	public class CheckOutController : Controller
	{
		private readonly EticaretVeritabaniContext _context;
		private readonly ApplicationDbContext _userContext;

		const string promosyonKodu = "Free";
		private ShoppingCart shoppingCart;


		public CheckOutController(EticaretVeritabaniContext eticaretVeritabaniContext, ApplicationDbContext context)
		{
			_userContext = context;
			_context = eticaretVeritabaniContext;
			shoppingCart = new ShoppingCart();
			shoppingCart._context = eticaretVeritabaniContext;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult AddressAndPayment()
		{
			//*ödeme tarafı için login olan kullanıcıya, sistemden ürettiğimiz random kodu mail olarak iletiriz. kullanıcıya giden kodu tabloda kod, username ve gönderilme tarihi olarak tutarız. 
			//*kod gönderildikten sonra 2 dk içerisinde eğer kullanıcı bizim ona gönderdiğimiz kodu zaman dolmadan dogru bir şekilde girebilmişse ödemeyi almış sayarız.

			string email = User.Identity.Name; //login olan kullanıcının bilgisi

			//yeni bir tablo oluşturalım.+ (UserPasswordHistories)
			HelperClass hc = new HelperClass(_userContext, _context);
			string sifre = hc.RandomSifreOlustur();

			UserPaymentCodeHistory history = new UserPaymentCodeHistory();
			history.SendDate = DateTime.Now;
			history.ExpireDate = history.SendDate.AddMinutes(2);
			history.UserName = email;
			history.Code = sifre;
			history.IsPaymentReceived = false;
			_context.UserPaymentCodeHistories.Add(history);
			_context.SaveChanges();


			return View();
		}


		public IActionResult KrediKartiEkle()
		{
			return View();
		}

		[HttpPost]
		public IActionResult KrediKartiEkle(CreditCart creditCart)
		{
			_context.CreditCarts.Add(creditCart);
			_context.SaveChanges();
			return RedirectToAction("AddressAndPayment");
		}
		public IActionResult AddressAndPayment2()
		{

			return View();
		}

		[HttpPost]
		public IActionResult AddressAndPayment2(Order order)
		{
			HelperClass helperClass = new HelperClass(_userContext, _context);

			order.UserName = User.Identity.Name;
			order.Total = Decimal.Parse(HttpContext.Session.GetString("alisverisTutari"));
			order.OrderDate = DateTime.Now;
			_context.Orders.Add(order);
			_context.SaveChanges();


			//OrderDetails sayfasına da kayıt eklememiz gerekir.
			string albumids = HttpContext.Session.GetString("albumIds");

			int adet = helperClass.ArananKarakterAdedi(albumids, ';');
			if (adet > 1)
			{
                //1den fazla album var.
                var cart = shoppingCart.GetCart(this.HttpContext);
                var AlbumIds=albumids.Split(';'); //5 3 2 
				foreach (var item in AlbumIds)
				{
					if (!String.IsNullOrWhiteSpace(item)) 
					{
                        OrderDetail orderDetail = new OrderDetail();
                        orderDetail.AlbumId = int.Parse(item);
                        var albums = _context.Carts.Where(satir => satir.CartId == cart.ShoppingCartId).OrderByDescending(satir => satir.DateCreated).FirstOrDefault();
                        orderDetail.Quantity = albums.Count;
                        var album = _context.Albums.Where(satir => satir.AlbumId == orderDetail.AlbumId).FirstOrDefault();

                        orderDetail.Album = album;
                        orderDetail.UnitPrice = album.Fiyat;
                        orderDetail.OrderId = order.OrderId;
                        _context.OrderDetails.Add(orderDetail);
                        _context.SaveChanges();
                    }
                  
                }

			}
			else
			{
				var cart = shoppingCart.GetCart(this.HttpContext);

				//9;
				//tek kayıt var ekle
				OrderDetail orderDetail = new OrderDetail();
				orderDetail.AlbumId = int.Parse(albumids.Split(';')[0]);
				var albums = _context.Carts.Where(satir => satir.CartId == cart.ShoppingCartId).OrderByDescending(satir => satir.DateCreated).FirstOrDefault();
				orderDetail.Quantity = albums.Count;
				var album = _context.Albums.Where(satir => satir.AlbumId == orderDetail.AlbumId).FirstOrDefault();

				orderDetail.Album = album;
				orderDetail.UnitPrice = album.Fiyat;
				orderDetail.OrderId = order.OrderId;
				_context.OrderDetails.Add(orderDetail);
				_context.SaveChanges();
			}




			return View();
		}


		public IActionResult IsValid(string adsoyad, string kartNumarasi, string dateM, string dateY, string cvv)
		{
			HelperClass helperClass = new HelperClass(_userContext, _context);

			kartNumarasi = helperClass.BosluklariSil(kartNumarasi);

			string sonkullanma = dateM + "/" + dateY;
			//girilen AdSoyad ve KartNumarası ve Sonkullanma ve CVV bilgileriyle eşleşen bir kayıt var mı? varsa içindeki limit ne? 
			//toplam alışveriş tutarı karttaki limitten az ise alışveriş gerçekleşecek, değilse hata
			var dbdenGelen = _context.CreditCarts.Where(satir => satir.AdSoyad == adsoyad && satir.KartNumarasi == kartNumarasi && satir.SonKullanma == sonkullanma && satir.CVV == cvv).FirstOrDefault();

			if (dbdenGelen == null)
			{
				//kart yok
				return Json("hata");
			}
			else
			{
				var cartItems = shoppingCart.GetCartItems(this.HttpContext);
				var aliverisTutari = cartItems.GetTotal();
				if (aliverisTutari <= dbdenGelen.KullanılabilirLimit)
				{
					//alışveriş tutarı<kart limiti
					return Json("ok");
				}
				else
				{
					return Json("hata");
				}
			}
		}
	}
}

