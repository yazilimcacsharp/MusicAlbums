using Eticaret.Data;
using Eticaret.Models;

namespace Eticaret.Helper
{
    public class ArtistHelper
    {
        private readonly EticaretVeritabaniContext _context;

        public ArtistHelper( EticaretVeritabaniContext context)
        {
            _context = context;
                
        }
        //veritabanındaki Artist tablosuna verileri eklesin

        public void ArtistVerileriniDoldur()
        {
            if (_context.Artists.ToList().Count == 0)
            {
                //ekleme işlemi çalışacak
                List<Artist> artist = new List<Artist>();
                artist.Add(new Artist() { Name = "Barış Manço", FotoPath = "https://mir-s3-cdn-cf.behance.net/project_modules/hd/32a36e32365535.567da68088b7d.jpg" });
                artist.Add(new Artist() { Name = "Sezen Aksu", FotoPath = "https://lastfm.freetls.fastly.net/i/u/avatar170s/1bb2fafac2acaf2e7a741d0caa9340e0" });
                artist.Add(new Artist() { Name = "Cem Karaca", FotoPath = "https://pbs.twimg.com/profile_images/1705486399/image.jpg" });
                artist.Add(new Artist() { Name = "Zeki Müren", FotoPath = "https://i4.hurimg.com/i/hurriyet/75/750x422/5d477b0d67b0a90c80e13b1b" });
                artist.Add(new Artist() { Name = "Ajda Pekkan", FotoPath = "https://i4.hurimg.com/i/hurriyet/75/750x422/5d70c10f7af5072c2cd1fd05" });
                artist.Add(new Artist() { Name = "Müslüm Gürses", FotoPath = "https://i2.cnnturk.com/i/cnnturk/75/646x0/5c86ab9b1da0a973ac27fa52" });

                foreach (var item in artist)
                {
                    _context.Artists.Add(item);
                }
                _context.SaveChanges();
            }            
        }
    }
}
