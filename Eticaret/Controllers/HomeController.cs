using Eticaret.Data;
using Eticaret.Helper;
using Eticaret.Models;
using Eticaret.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Eticaret.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly EticaretVeritabaniContext _eticaretVeriTabaniContext;
		private ShoppingCart shoppingCart;
		public HomeController(ILogger<HomeController> logger, ApplicationDbContext applicationDbContext, EticaretVeritabaniContext eticaretContext)
		{
			_logger = logger;
			_applicationDbContext = applicationDbContext;
			_eticaretVeriTabaniContext = eticaretContext;
			shoppingCart = new ShoppingCart();
			shoppingCart._context = eticaretContext;
		}

		//Home/Test : urlde bu sırayla : controllerAdi/metotAdi
		public IActionResult Test()
		{
			return View();
		}
		public IActionResult Index()
		{

			//Session.SetString("anahtarKelime","içine koyulacak Deger");
			HttpContext.Session.SetString("siteAdi", "infotech");  //set: değer atamak, get: değeri okumak
																

			var cart = shoppingCart.GetCart(this.HttpContext);
			HttpContext.Session.SetString("adet", cart.GetCount().ToString());

			ViewData["SepetAdet"] = HttpContext.Session.GetString("adet");
			HttpContext.Session.Clear(); //sessiondaki değerleri sıfırlamak için kullanılan kod.


			////mail çalışıyor mu deneyi
			//try
			//{
			//	MailSender sender = new MailSender(_applicationDbContext);
			//	var task = sender.SendEmailAsync("gonderilecekMailAdresi@gmail.com", "asansörümüzde sıkıntı var", "1.kattan asansöre bindim, kaldım");


			//	//toplu mail atmak istersek bu durumda aşağıdaki kodu çalıştırmamız.
			//	//MailSender sender = new MailSender(_applicationDbContext);
			//	//foreach (var item in MailAdresleriToplulugu)
			//	//{					
			//	//	var task = sender.SendEmailAsync(item, "asansörümüzde sıkıntı var", "1.kattan asansöre bindim, kaldım");
			//	//}
			//}
			//catch (Exception ex)
			//{
			//	string sonuc = ex.Message;
			//}





			//Turler eklenecek.
			HelperClass helperClass = new HelperClass(_applicationDbContext,_eticaretVeriTabaniContext);
			var liste = helperClass.TurDatalariniEkle();
			if (_eticaretVeriTabaniContext.Turs.Count() == 0)
			{
				foreach (var item in liste)
				{
					_eticaretVeriTabaniContext.Turs.Add(item);
				}
				_eticaretVeriTabaniContext.SaveChanges();
			}

			var turler = _eticaretVeriTabaniContext.Turs.ToList();

			//artist verileri burada eklenecek.
			ArtistHelper helper = new ArtistHelper(_eticaretVeriTabaniContext);
			helper.ArtistVerileriniDoldur();


			AlbumHelper albumHelper = new AlbumHelper(_eticaretVeriTabaniContext);
			albumHelper.AddAlbums();

			//MailSender sender = new MailSender(_applicationDbContext);
			//sender.SendEmailAsync("yazilimca32@gmail.com", "biz attık", "yeni test deneme");
			//sender.SendEmailAsync("tuba.ozt@hotmail.com", "biz attık", "yeni test deneme");

			CokSatanTurViewModel models = new CokSatanTurViewModel();

			models.Turs = turler;

			//çok satan albumleri tespit edip o albumleri albums içerisine dolduralım.

			//select top 5 AlbumId,sum(Quantity) from Orderdetails group by AlbumId
			var cokSatanlar = _eticaretVeriTabaniContext.OrderDetails
				.GroupBy(satir => satir.AlbumId)
				.Select(grup => new { AlbumId = grup.Key, ToplamMiktar = grup.Sum(satir => satir.Quantity) })
				.OrderByDescending(s => s.ToplamMiktar)
				.Take(5)
				.ToList();

			List<Album> albums = new List<Album>();
			foreach (var item in cokSatanlar)
			{
				albums.AddRange(_eticaretVeriTabaniContext.Albums.Where(satir => satir.AlbumId == item.AlbumId).ToList());
			}

			
			models.Albums = albums;



			return View(models);
		}

		[HttpPost]
		public IActionResult Index(int id)
		{
			
			return View("Index", "Home");
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[Authorize(Roles = "Admin")]
		public IActionResult AdminSayfasi() //Admin panelli bir html sayfa açılırdı.
		{
			return View();
		}

		public IActionResult TCNo()
		{
			return View();
		}


		public IActionResult Hakkimizda()
		{

			var cart = shoppingCart.GetCart(this.HttpContext);
			HttpContext.Session.SetString("adet", cart.GetCount().ToString());
			ViewData["SepetAdet"] = HttpContext.Session.GetString("adet");


			return View();
		}

		public IActionResult KategoriyeGoreAlbumler(int id) //parametre olarak gelen değer turid bilgisi.
		{
			string aaa = HttpContext.Session.GetString("siteAdi");
			//albumlere git, yukarıdaki id ile eşleşen albumleri bul, listele ve sayfada göster.
			List<Album> albumler = _eticaretVeriTabaniContext.Albums.Include(satir => satir.Tur).Include(satir => satir.Artist).Where(satir => satir.TurId == id).ToList();
			
			var cart = shoppingCart.GetCart(this.HttpContext);
			HttpContext.Session.SetString("adet", cart.GetCount().ToString());
			ViewData["SepetAdet"] = HttpContext.Session.GetString("adet");

			return View(albumler); //albumler datası ile view'ı açmalı
		}


		//album detayına gitmek istersek 
		public IActionResult Details(int id) //parametre olarak gelen değer albumid bilgisi.
		{
			//albumlere git, yukarıdaki id ile eşleşen albumleri bul, listele ve sayfada göster.
			Album secilenAlbum = _eticaretVeriTabaniContext.Albums.Where(satir => satir.AlbumId == id).FirstOrDefault();

			var cart = shoppingCart.GetCart(this.HttpContext);
			HttpContext.Session.SetString("adet", cart.GetCount().ToString());
			ViewData["SepetAdet"] = HttpContext.Session.GetString("adet");

			return View(secilenAlbum);
		}

		public IActionResult CreateOrder(int id)
		{			
			//cart.CreateOrder()
			return View();
		}

		////hangi ürün,kaç tane, OrderId bilgisi GUid ile oluşturulup eklensin.
		//public IActionResult CreateOrder(int AlbumId, int Quantity,string fiyat)
		//{
		//	//OrderDetails tablosuna kayıt eklenecek.
		//	OrderDetail orderDetail = new OrderDetail();
		//	orderDetail.OrderId = 1;
		//	orderDetail.Quantity = Quantity;
		//	orderDetail.AlbumId = AlbumId;
		//	orderDetail.UnitPrice =Convert.ToDecimal(fiyat);

		//	_eticaretVeriTabaniContext.OrderDetails.Add(orderDetail);
		//	_eticaretVeriTabaniContext.SaveChanges();

		//	return Json("ok");

		//}
	}
}
