using Eticaret.Data;
using Eticaret.Models;

namespace Eticaret.Helper
{
    public class AlbumHelper
    {
        //Album nesneleri oluşturulacak.
        //dbye kaydedilecek

        private readonly EticaretVeritabaniContext musicStoreContext;

        public AlbumHelper(EticaretVeritabaniContext context)
        {
            musicStoreContext = context;
        }

        public void AddAlbums()
        {
            if (musicStoreContext.Albums.ToList().Count == 0)
            {
                var liste = new List<Album>
                {
                    new Album { Baslik = "The Best Of Men At Work", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"),AlbumArtUrl = "/images/default.jpeg" },
                    new Album { Baslik = "Worlds", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Jazz"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "For Those About To Rock We Salute You", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Let There Be Rock", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Balls to the Wall", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Restless and Wild", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Big Ones", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Quiet Songs", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Jazz"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Jagged Little Pill", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Facelift", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Frank", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Pop"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Warner 25 Anos", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Jazz"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Audioslave", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "The Best Of Billy Cobham", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Jazz"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Chronicle, Vol. 1", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Chronicle, Vol. 2", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Into The Light", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Come Taste The Band", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Sezen Aksu"), AlbumArtUrl = "/images/default.jpeg" },
new Album { Baslik = "Deep Purple In Rock", Tur = musicStoreContext.Turs.Single(g => g.TurAdi == "Rock"), Fiyat = 8.99M, Artist = musicStoreContext.Artists.Single(a => a.Name == "Cem Karaca"), AlbumArtUrl = "/images/default.jpeg" },

                };


                foreach (var item in liste)
                {
                    item.Ad = item.Baslik;
                    musicStoreContext.Albums.Add(item);
                }
                musicStoreContext.SaveChanges();
            }
        }
    }
}