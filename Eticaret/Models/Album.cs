using System.ComponentModel.DataAnnotations;

namespace Eticaret.Models
{
    public class Album
    {

        //Album
        //id
        //ad
        //kapakgorseli
        //başlık
        //fiyat
        //isActive
        public int AlbumId { get; set; }

        public string Ad { get; set; }
        public string AlbumArtUrl { get; set; }

        [Required(ErrorMessage ="Başlık zorunlu alandır") ]
        [StringLength(50)]   //nvarchar(50)
        public string Baslik { get; set; }

        [Range(0.01,500.00,ErrorMessage ="Girilen fiyat uygun aralıkta değil. 0.01 ile 500 aralığında olmalıydı")]
        public decimal Fiyat { get; set; }
        public bool AktifMi { get; set; }


        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }


		public int TurId { get; set; }
		public virtual Tur Tur { get; set; }

        //OrderDetails eklenecek.



	}
}
