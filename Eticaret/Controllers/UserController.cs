using Eticaret.Data;
using Eticaret.Helper;
using Eticaret.Models;
using Eticaret.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using SQLitePCL;
using System;
using System.Security.Cryptography;

namespace Eticaret.Controllers
{
	public class UserController : Controller
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly EticaretVeritabaniContext eticaretVeritabaniContext;
		private readonly UserManager<IdentityUser> _userManager;
		public UserController(ApplicationDbContext applicationDbContext, UserManager<IdentityUser> userManager, EticaretVeritabaniContext ctx)
		{
			_applicationDbContext = applicationDbContext;
			eticaretVeritabaniContext = ctx;
			_userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}

		public IActionResult RolEkle()
		{
			return View();
		}

		[HttpPost]
		public IActionResult RolEkle(IdentityRole rolex)
		{
			//1.id değerinin otomatik artmaması durumundan dolayı, gidip tabloya en son kullanılan id değerini bulup, 1 artırıp yeni eklediğimizi o değerle dbye kaydetmek gerekecek.

			HelperClass hc = new HelperClass(_applicationDbContext, eticaretVeritabaniContext);
			int idDegeri = hc.sonIdAL();
			IdentityRole role = new IdentityRole();
			role.Id = idDegeri.ToString();
			role.Name = rolex.Name;
			_applicationDbContext.Add(role);
			_applicationDbContext.SaveChanges();
			return RedirectToAction("Albumler", "Album");
			//return RedirectToAction("Albumler"); //controllerı bulundugu sayfadan alır, User ... User/Albumler
			//return RedirectToAction("Albumler","Album"); // Album/Albumler
		}


		//Rollerin gösterileceği sayfayı ekleyelim.
		public IActionResult Roller()
		{
			List<IdentityRole> roller = _applicationDbContext.Roles.ToList();
			return View(roller); //Html sayfası açılacak.
		}

		//kullanıcıların bilgilerini 
		//kullanıcı Adı(mail), rol adları gösterilecek sekilde sayfayı oluşturalım.
		public IActionResult Kullanicilar()
		{
			//1.applicationDbContext üzerinden kullanıcıları alalım.(dbden)
			//2.rolleri alalım.
			//3.2sini birlikte gösterelim.


			//sql
			//select u.UserName,r.Name from AspNetUserRoles ur join AspNetUsers u
			//on ur.UserId = u.Id  join AspNetRoles r
			//on r.Id = ur.RoleId

			//lambda: .Where(satir=>satir.Id==3);

			//linq: language integrated query: dile entegre sorgulama yazma
			var model = from t in _applicationDbContext.Users
						join x in _applicationDbContext.UserRoles
						on t.Id equals x.UserId into userRoles
						from ur in userRoles.DefaultIfEmpty()
						join z in _applicationDbContext.Roles
						on ur.RoleId equals z.Id into roles
						from r in roles.DefaultIfEmpty()
						select new UserRole
						{
							//Id = Guid.NewGuid().ToString(),
							RolName = r != null ? r.Name : null,
							UserName = t.UserName
						};


			//2.yol : lambda ve join kullanımı
			//var model = _applicationDbContext.UserRoles.Join(_applicationDbContext.Roles,x => x.RoleId, z => z.Id, (x, z) => new { x, z })
			//	.Join(_applicationDbContext.Users, y => y.x.UserId, t => t.Id, (y, t) => new { y, t })
			//	.Select(result => new UserRole { RolName = result.y.z.Name, UserName = result.t.UserName });



			return View(model);

		}



		//kullanıcı eklemek için yeni bir sayfa oluşturalım.
		public IActionResult KullaniciEkle()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> KullaniciEkle(IdentityUser user)
		{
			//Users tablosundaki zorunlu alanları çıkartma işlemi      

			//email +
			//IdentityUser newUser = new IdentityUser();
			//newUser.Id = Guid.NewGuid().ToString(); //Id: otomatik artmıyor,: Guid vermek gerek.
			//										//Email : Username / normalizedUsername /normalizedEmail / aynı
			//newUser.Email = user.Email;
			//newUser.UserName = user.Email;
			//newUser.NormalizedUserName = user.Email.ToUpper().Replace("İ", "I");
			//newUser.NormalizedEmail = user.Email.ToUpper().Replace("İ", "I");
			//newUser.EmailConfirmed = false; //EmailConfirmed: default 0 olmalı
			//newUser.PhoneNumberConfirmed = false; //PhoneNumberConfirmed: 0
			//newUser.TwoFactorEnabled = false; //TwoFactorEnabled: 0
			//newUser.LockoutEnabled = true; //LockoutEnabled: 1
			//newUser.AccessFailedCount = 0; //AccessFailedCount: 0
			//newUser.PasswordHash = user.PasswordHash;
			//_applicationDbContext.Add(newUser);
			//_applicationDbContext.SaveChanges();



			//üyelik sisteminin register buttonuna tıklanınca çalışan kodu burada kullanırsak işimiz çözülür mü?
			user.UserName = user.Email;
			await _userManager.CreateAsync(user, user.PasswordHash);


			return RedirectToAction("Kullanicilar");
		}


		//kullanıcıyı ekledik fakat onaylı değil, admine kullanıcı onayı yapabilmesi için onay ekranı verelim. +
		public IActionResult KullaniciOnay()
		{
			var onaylanacaklar = _applicationDbContext.Users./*Where(satir => satir.EmailConfirmed == false)*/ToList();
			return View(onaylanacaklar);
		}

		//kullanıcı Onay sayfasına button ekleyelim. button onaylamak için kullanılsın.+
		//onay verecek kullanıcı buttona tıkladıgında gitsin backendde o kişiyi onaylasın.+



		public IActionResult Onay(string username)
		{
			try
			{
				IdentityUser dbdenGelen = _applicationDbContext.Users.Where(satir => satir.UserName == username).FirstOrDefault();
				dbdenGelen.EmailConfirmed = true;
				_applicationDbContext.Update(dbdenGelen);
				_applicationDbContext.SaveChanges();
				return Json("ok");
			}
			catch
			{
				return Json("hata");
			}

		}

		//button ekleyip şifre oluşturma kodunu da ekleyelim .++
		public async Task<IActionResult> RandomSifreOlusturma(string username)
		{


			//username bilgisinin dbde passwordü var mı eğer varsa şifre oluşturma yoksa, uyarı verip başka sayfaya yönlendirme
			IdentityUser dbdenGelen = _applicationDbContext.Users.Where(satir => satir.UserName == username).FirstOrDefault();
			if (dbdenGelen.PasswordHash != null)
			{
				return Json("password var");
			}
			else
			{
				HelperClass hc = new HelperClass(_applicationDbContext,eticaretVeritabaniContext);
				string sifre = hc.RandomSifreOlustur();
				//uretilen sifreyi dbde o kişinin password alanına ata.
				//MD5 mds=new MD5(); //md5 ya da sha vs. , hashing algorihtması kullanılarak sifre 3.kişiler tarafından gözükmeyecek hale getirilir.

				//üretilen şifreyi mail ile gönderme

				//MailSender sender = new MailSender(_applicationDbContext);
				//sender.SendEmailAsync("tuba.ozt@hotmail.com", "biz attık", "yeni test deneme");

				var result = await UpdateUser(dbdenGelen, sifre);

				//1.mail gönderme+
				MailSender sender = new MailSender(_applicationDbContext);

				sender.SendEmailAsync("32@gmail.com", "Password Recovery", "<!DOCTYPE html>\r\n<html>\r\n<head>\r\n    <meta charset=\"utf-8\" />\r\n    <title></title>\r\n</head>\r\n<body>\r\n  <p>" + "Şifreniz Güncellendi. Yeni Şifreniz: " + sifre + "</p> <img src=\"https://www.shutterstock.com/image-vector/password-theft-cracking-cybercrime-hacker-260nw-2210091473.jpg\" style=\"width:200px;height:200px;\" />\r\n    <h1>Şifre Onaylama Ekranı</h1>\r\n    <p>Şifrenizi onaylamak için <a href=\"https://www.youtube.com/\" target=\"_blank\">tıklayınız</a></p>\r\n\r\n</body>\r\n</html>");

				//2.userPasswordHistories tablosuna bu şifreyi kaydedelim.
				UserPasswordHistory passwordHistory = new UserPasswordHistory();
				passwordHistory.CreatedDate = DateTime.Now;
				passwordHistory.ExpireDate = passwordHistory.CreatedDate.AddMinutes(1); //1 DK İÇİNDE KENDİSİNE İLETİLEN ŞİFREYİ GİRMELİDİR.
				passwordHistory.IsActive = false;
				passwordHistory.Mail = dbdenGelen.Email;
				passwordHistory.Password = sifre;
				_applicationDbContext.UserPasswordHistories.Add(passwordHistory);
				_applicationDbContext.SaveChanges();




				return Json("ok");


			}
		}



		//kullanıcnın şifre güncelleme kısmını üyelik sistemi tarafından güncellenmesini sağlamak içindir.
		public async Task<IActionResult> UpdateUser(IdentityUser user, string sifre)
		{

			//1.usermanager üzerindeng git o userI bul.
			var userx = await _userManager.FindByIdAsync(user.Id);

			if (userx == null)
			{
				return NotFound("kullanıcı bulunamadı");
			}

			//yeni ayarlayıp 


			var yeniPasswordHashli = _userManager.PasswordHasher.HashPassword(user, sifre);
			userx.PasswordHash = yeniPasswordHashli;

			var result = await _userManager.UpdateAsync(userx);

			if (!result.Succeeded)//Succeeded değilse ! : olumsuzluk anlamını taşır
			{
				return BadRequest("şifre güncellenemedi");
			}

			return View();

		}















		//RolAta
		//kullanıcıya rol atama işlemi yapılacak.
		//sayfada önce kullanıcı seçilir. (kullanıcıları çekip dropdowna doldur.)
		//roller seçilir (roller dropdowna doldurma)
		//buttona tıklanınca AspNetUserRoles tablosuna o kullanıcının idsini ve Seçilen rolün idsini ekle

		public IActionResult KullaniciRolAta()
		{
			//kullanıcıları çekip dropdowna doldur.
			List<IdentityUser> kullanicilar = _applicationDbContext.Users.ToList();
			List<IdentityRole> roller = _applicationDbContext.Roles.ToList();
			ViewBag.kullanicilar = new SelectList(kullanicilar, "Id", "UserName");
			ViewBag.roller = new SelectList(roller, "Id", "Name");
			return View();
		}

		[HttpPost]
		public IActionResult KullaniciRolAta(UserRol userRol)
		{

			IdentityUserRoleY identityUserRole = new IdentityUserRoleY();
			identityUserRole.Id = Guid.NewGuid().ToString();
			identityUserRole.UserId = userRol.UserId;
			identityUserRole.RoleId = userRol.RolId.ToString();

			_applicationDbContext.UserRoles.Add(identityUserRole);
			_applicationDbContext.SaveChanges();
			return RedirectToAction("Kullanicilar");
		}

		public IActionResult Delete(int id)
		{
			var silinecekYetkiDetay = _applicationDbContext.UserRolesY.First(satir => satir.Id == id.ToString());
			return View(silinecekYetkiDetay);
		}




		//Userların hepsinin rolleri 3 olarak değişsin.
		public IActionResult TopluGuncelleme()
		{
			//alttaki kod tablodaki login olan kişide dahil olmak üzere, herkesi 3 rolüne güncelledi. (login olan kişi hariç diğer kullanıcıların rolünü değiştir demek lazım.

			string loginOlanKisiId = _applicationDbContext.Users.Where(satir => satir.UserName == User.Identity.Name).FirstOrDefault().Id;

			_applicationDbContext.Database.ExecuteSqlRaw("update AspNetUserRoles Set RoleId='3' where UserId!='" + loginOlanKisiId + "'");

			return Json("ok");
		}


		////buttona tıklanınca httppost işlemi olur, onun kodunu da yazmamız gerekir.
		//[HttpPost]
		//public IActionResult DeleteConfirmed(int AlbumId)
		//{
		//	////1.idden kimi sileceğimizi bulalım.
		//	//Album silinecekAlbum=_context.Albums.Where(satir => satir.AlbumId == AlbumId).FirstOrDefault();
		//	//_context.Remove(silinecekAlbum);
		//	//_context.SaveChanges();
		//	//return RedirectToAction("Albumler");


		//	//aktifmi alanını 0 olarak güncelleyecek kod
		//	Album silinecekAlbum = _context.Albums.Where(satir => satir.AlbumId == AlbumId).FirstOrDefault();
		//	silinecekAlbum.AktifMi = false;
		//	_context.Update(silinecekAlbum);
		//	_context.SaveChanges();
		//	return RedirectToAction("Albumler");
		//}




		public IActionResult SifreOnay(string username)
		{
			HttpContext.Session.SetString("username", username);
			return View();
		}

		[HttpPost]
		public IActionResult SifreOnay(string usernamex, string password)
		{
			string username = HttpContext.Session.GetString("username");
			//-SİFRENİN İPTAL OLMA ZAMANI OLMALI,++
			//-Kullanıcının mail ile iletilen şifreyi girmesi lazım.  eğer doogru girerse, hala zaman geçmemişse bu durumda o kullanıcının üyeliği aktif duruma getirilir.
			var userBilgileri = _applicationDbContext.UserPasswordHistories.Where(satir => satir.Mail == username).OrderByDescending(satir => satir.CreatedDate).FirstOrDefault();

			if (password == userBilgileri.Password && userBilgileri.ExpireDate >= DateTime.Now)
			{
				userBilgileri.IsActive = true;
			}
			_applicationDbContext.SaveChanges();

			return View();
		}



		public IActionResult OdemeYap(string code)
		{
			var dbdenGelen=eticaretVeritabaniContext.UserPaymentCodeHistories.Where(satir => satir.UserName == User.Identity.Name).OrderByDescending(satir => satir.SendDate).FirstOrDefault();

			if (dbdenGelen.Code == code && DateTime.Now<=dbdenGelen.ExpireDate)
			{
				dbdenGelen.IsPaymentReceived = true; //zamanında ve dogru girilmişse dbde aktif yaptık
				eticaretVeritabaniContext.Update(dbdenGelen);
				eticaretVeritabaniContext.SaveChanges();
				return Json("ok");
			}
			else
			{
				return Json("hata");
			}
		}



		public IActionResult PaymentSuccess()
		{
			//random yapısını kullanarak, sayısal değerlerden oluşan bir kod üretelim.
			//bu üretilen sipariş numaralarını tabloya yazalım. 
			//yeni uretilmiş olan kod daha önce üretilmiş ve tabloda varsa(tablo oluşacak)  farklı bir değer olması için tekrar üretelim.
			//pttkargo tarafına bu değer iletilir. (kargo bilgilerini kaydettiğimiz tabloya da eklemek gerekir.)
			//bu değer kullanıcıya mail-sms olarak gönderilir.

			HelperClass helperClass = new HelperClass(_applicationDbContext,eticaretVeritabaniContext);
			string siparisNo=helperClass.RandomSiparisNoOlustur();
			if (helperClass.SiparisNoVarmi(siparisNo)==false)
			{
				//siparisno yok. 

				//daha önce böye bir sipariş no üretilmemiş
				OrderNumberHistory orderNumberHistory = new OrderNumberHistory();
				orderNumberHistory.OrderNumberCode = siparisNo;

				eticaretVeritabaniContext.OrderNumberHistories.Add(orderNumberHistory);
				eticaretVeritabaniContext.SaveChanges();
			}
			else
			{
				//varsa yeni kod üret.
				siparisNo = helperClass.RandomSiparisNoOlustur();
			}

			ViewData["SiparisNo"] = siparisNo;






			return View();
		}
	}
}
