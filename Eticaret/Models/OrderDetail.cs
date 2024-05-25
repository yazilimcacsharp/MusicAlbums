namespace Eticaret.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public virtual Order Order { get; set; }

        public int AlbumId { get; set; }  //siparişte sattgımız urun hangi urun
        public virtual Album Album { get; set; }

        public int Quantity { get; set; } //sipariş Adet bilgisini tutar.
        public decimal UnitPrice { get; set; } //eklenen ürünün fiyatı


    }
}
