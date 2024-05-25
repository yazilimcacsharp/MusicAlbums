using Eticaret.Models;
using Microsoft.EntityFrameworkCore;


namespace Eticaret.Data
{
    public class EticaretVeritabaniContext: DbContext
    {
        public EticaretVeritabaniContext(DbContextOptions<EticaretVeritabaniContext> options)
         : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Tur> Turs { get; set; }
        public DbSet<Artist> Artists { get; set; }

        //order ve order detail bilgilerinin tablolarını oluşturtmak için kodu yazalım.
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Cart> Carts { get; set; } //sepete eklenen ürünleri db tarafında tutmak için tanımlandı.
        public DbSet<UserPaymentCodeHistory> UserPaymentCodeHistories { get; set; } 
        public DbSet<OrderNumberHistory> OrderNumberHistories { get; set; } 
        public DbSet<CreditCart> CreditCarts { get; set; } 


    }
}
