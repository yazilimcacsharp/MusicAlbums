using Eticaret.Data;
using Eticaret.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Eticaret.Helper
{
	public class HelperClass
	{
		private readonly ApplicationDbContext _applicationDbContext;
		private readonly EticaretVeritabaniContext _ctx;

        public HelperClass(ApplicationDbContext applicationDbContext,EticaretVeritabaniContext eticaretVeritabaniContext)
        {
            _applicationDbContext = applicationDbContext;	
			_ctx = eticaretVeritabaniContext;
        }


        public bool IsValidTcno(long value)
		{
			string deger = value.ToString();
			if (deger.Length == 11)
			{
				int bir = Convert.ToInt32(deger.Substring(0, 1)),
					iki = Convert.ToInt32(deger.Substring(1, 1)),
					uc = Convert.ToInt32(deger.Substring(2, 1)),
					dort = Convert.ToInt32(deger.Substring(3, 1)),
					bes = Convert.ToInt32(deger.Substring(4, 1)),
					alti = Convert.ToInt32(deger.Substring(5, 1)),
					yedi = Convert.ToInt32(deger.Substring(6, 1)),
					sekiz = Convert.ToInt32(deger.Substring(7, 1)),
					dokuz = Convert.ToInt32(deger.Substring(8, 1)),
					on = Convert.ToInt32(deger.Substring(9, 1)),
					onbir = Convert.ToInt32(deger.Substring(10, 1));

				int son = ((((bir + uc + bes + yedi + dokuz) * 7) - (iki + dort + alti + sekiz)) % 10);
				if (son == on)
				{
					int ilkonhanetoplam = bir + iki + uc + dort + bes + alti + yedi + sekiz + dokuz + on;
					int onnbir = ilkonhanetoplam % 10;
					return onbir == onnbir;
				}
			}
			return false;
		}


		//dbye git son idyi al.
		public int sonIdAL()
		{
			string id=_applicationDbContext.Roles.Select(satir => satir.Id).Max();
			int idm= Convert.ToInt32(id);
			return ++idm;
        }


		public string RandomSifreOlustur()
		{
			string sifre = string.Empty;
			Random random=new Random();
			for(int i = 0; i <= 10; i++)
			{
				int uretilenDeger = random.Next(65, 91);
				sifre += (char)uretilenDeger;
			}
			return sifre;
		}

		public string RandomSiparisNoOlustur()
		{
			string sifre="";
			Random random=new Random();
			for (int i = 0; i < 6; i++) // 6 haneli şifre
			{
				sifre += random.Next(0, 10); //0-9 aralıgında bir değer üretecek
			}
			return sifre;
		}

		public bool SiparisNoVarmi(string siparisNo)
		{
			var dbdenGelen=_ctx.OrderNumberHistories.Where(c => c.OrderNumberCode == siparisNo).FirstOrDefault();
			if (dbdenGelen == null)
				return false;
			else
				return true;
		}
		


		//turs tablosuna kategoriler eklensin diye yazılan metot.
		public List<Tur> TurDatalariniEkle()
		{
            List<Tur> turs = new List<Tur>();
			turs.Add(new Tur() { TurAdi = "Arabesk", FotografYolu = "/images/arabesk.jpg" });
			turs.Add(new Tur() { TurAdi = "Pop", FotografYolu = "/images/pop.png" });
			turs.Add(new Tur() { TurAdi = "Rock", FotografYolu = "/images/rock.jpg" });
			turs.Add(new Tur() { TurAdi = "Rap", FotografYolu = "/images/rap.jpg" });
			turs.Add(new Tur() { TurAdi = "Türk Sanat Müziği", FotografYolu = "/images/tsm.jpg" });
			turs.Add(new Tur() { TurAdi = "Türk Halk Müziği", FotografYolu = "/images/thm.jpg" });
			turs.Add(new Tur() { TurAdi = "Jazz", FotografYolu = "/images/jazz.jpg" });
			turs.Add(new Tur() { TurAdi = "90lar", FotografYolu = "/images/default.jpg" });
			turs.Add(new Tur() { TurAdi = "80ler", FotografYolu = "/images/default.jpg" });
			turs.Add(new Tur() { TurAdi = "70ler", FotografYolu = "/images/default.jpg" });
			return turs;
		}

		public string BosluklariSil(string deger)
		{
			string result = string.Empty;
			//4444 3333 2222 1111:  ..... 4444333322221111 boşluksuz hale ceviren metot.
			string[] degerler=deger.Split(' ');
			foreach (var item in degerler)
			{
				result += item;
			}
			return result;
		}


		//CartItems içerisindeki AlbumId bilgilerini bir stringe dolduracak metot.
		public string GetAlbumIds(List<Cart> degerler)
		{
			string result = string.Empty;
			foreach (var item in degerler)
			{
				result += item.AlbumId;
				result += ";";
			}
			return result;
		}


		public int ArananKarakterAdedi(string deger,char arananKarakter)
		{
			int adet = 0;
			foreach (var item in deger)
			{
				if(item==arananKarakter)
				{
					adet++;
				}
			}
			return adet;
		}


	}
}
