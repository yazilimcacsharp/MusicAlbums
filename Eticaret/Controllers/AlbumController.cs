using Eticaret.Data;
using Eticaret.Helper;
using Eticaret.Models;
using Eticaret.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Eticaret.Controllers
{
    public class AlbumController : Controller
    {
        private readonly EticaretVeritabaniContext _context;
        private readonly ApplicationDbContext _applicationDbContext;



        public AlbumController(EticaretVeritabaniContext context, ApplicationDbContext appcontext)
        {
            _context = context;
            _applicationDbContext = appcontext;
        }
        public IActionResult Index()
        {



            return View();
        }

        public IActionResult AlbumEkle()
        {
            //Artist datalarını tablodan getirip, bunları SelectList yapısında ViewBag ile datayı view tarafındaki dropdowna taşıyalım.
            ViewBag.artists = new SelectList(_context.Artists.OrderBy(satir => satir.Name), "ArtistId", "Name");
            ViewBag.turler = new SelectList(_context.Turs.OrderBy(satir => satir.TurAdi), "TurId", "TurAdi");
            return View();
        }


        public IActionResult FotoYukle()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AlbumEkle(Album album, IFormFile file)
        {
            album.AlbumArtUrl = "/uploads/" + file.FileName + ";" + User.Identity.Name + ";" + Path.GetExtension(file.FileName);
            _context.Add(album);
            _context.SaveChanges();


            //yetkiyi kontrol etmek istersek
            //album.AlbumArtUrl = "/uploads/" + file.FileName + ";n3@gmail.com;" + Path.GetExtension(file.FileName);
            //if (album.AlbumArtUrl.Contains(User.Identity.Name))
            //{
            //    _context.Add(album);
            //    _context.SaveChanges();
            //}


            return RedirectToAction("Albumler");
        }

        //Albumler listesi oluşacak
        public IActionResult Albumler()
        {

            List<Album> albumlerHepsi = _context.Albums.Include("Artist").Include("Tur").Where(satir => satir.AktifMi == true).ToList();

            return View(albumlerHepsi);
        }

        public IActionResult Edit(int id)
        {
            var albumdetay = _context.Albums.First(satir => satir.AlbumId == id);
            return View(albumdetay);
        }

        [HttpPost]
        public IActionResult Edit(int id, Album album,IFormFile file)
        {
            var albumdetay = _context.Albums.First(satir => satir.AlbumId == id);
            albumdetay.Baslik = album.Baslik;
            albumdetay.Ad = album.Ad;
            albumdetay.Fiyat = album.Fiyat;
            albumdetay.AlbumArtUrl = "/uploads/" + file.FileName + ";" + User.Identity.Name + ";" + Path.GetExtension(file.FileName);
            //albumdetay.AlbumArtUrl = album.AlbumArtUrl;
            albumdetay.AktifMi = album.AktifMi;
            _context.Update(albumdetay);
            _context.SaveChanges();
            return RedirectToAction("Albumler");
        }



        //1.ekranı gösterme : /Album/Details : HttpGet : Browserdan yapılan cagrı gettir.
        public IActionResult Details(int id)
        {
            var albumdetay = _context.Albums.First(satir => satir.AlbumId == id);
            return View(albumdetay);
        }




        //yeni bir sayfa oluşturulacaksa 2 tane metot yazıyoruz.
        //1.ekranı gösterme : /Album/Details : HttpGet : Browserdan yapılan cagrı gettir.
        //2.buttona tıklanınca yapılacak kodlar
        public IActionResult Delete(int id)
        {
            var albumdetay = _context.Albums.First(satir => satir.AlbumId == id);
            return View(albumdetay);
        }


        //buttona tıklanınca httppost işlemi olur, onun kodunu da yazmamız gerekir.
        [HttpPost]
        public IActionResult DeleteConfirmed(int AlbumId)
        {
            ////1.idden kimi sileceğimizi bulalım.
            //Album silinecekAlbum=_context.Albums.Where(satir => satir.AlbumId == AlbumId).FirstOrDefault();
            //_context.Remove(silinecekAlbum);
            //_context.SaveChanges();
            //return RedirectToAction("Albumler");


            //aktifmi alanını 0 olarak güncelleyecek kod
            Album silinecekAlbum = _context.Albums.Where(satir => satir.AlbumId == AlbumId).FirstOrDefault();
            silinecekAlbum.AktifMi = false;
            _context.Update(silinecekAlbum);
            _context.SaveChanges();
            return RedirectToAction("Albumler");
        }


        //SilinenleriGetir : albumler tablosundaki bütün kayıtlara baksın, 0 olanları 1 yapsın.
        public IActionResult TopluSilme()
        {
            List<Album> pasifler = _context.Albums.Where(satir => satir.AktifMi == false).ToList();
            foreach (var item in pasifler)
            {
                item.AktifMi = true;
                _context.Update(item);
                _context.SaveChanges();
            }
            return RedirectToAction("Albumler");
        }

        [HttpPost]
        public IActionResult TcNoKontrol(long id)
        {
            HelperClass helper = new HelperClass(_applicationDbContext, _context);
            if (helper.IsValidTcno(id))
            {
                return Json("ok");
            }
            else
            {
                return Json("yok");
            }
        }

    }
}
