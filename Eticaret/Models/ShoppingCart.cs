using Eticaret.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace Eticaret.Models
{
	//sipariş oluşturma,
    //sepetteki ürünleri getirme,       
	public partial class ShoppingCart
	{
        //EticaretVeritabaniContext örnekleme
        public EticaretVeritabaniContext _context { get; set; } //veritabanı baglantısı için.entity framework aracılıgı ile dbye baglanmak istediğimizde context yapısını kullanırız.
		public string ShoppingCartId { get; set; }  //kullanıcının alışveriş sepeti için unique benzersiz bir tanımlayıcı

		public const string CartSessionKey="CartId";  //const: sabit anlamına gelir. oturum içinde sepeti tanımlamak için kullanılan sabit bir anahtardır
           

        //GetCart(): HttpContext aracılıgı ile sepetteki ürünleri getirecek.
        //kullanıcıya özgü alışveriş sepeti nesnesi oluşturur. http protokolü üzerinden istekte bulunan sepete ürün ekleyen kişinin bilgilerine erişir.
        public ShoppingCart GetCart(HttpContext context)
        {
            var cart = new ShoppingCart();
            cart._context=_context;
            cart.ShoppingCartId = cart.GetCartId(context); //login ise maili, değilse guid bilgisi verir.
            return cart;
        }

        //o an sepetine ürün ekleyen kişi login mi değil mi kontrol eden metot. 
        //eğer login ise CartSessionKey içerisine kişinin mailini, login değilse sistem tarafından bir Guid değeri üretilip o değeri CartSessionKey'a atayan ve CartSessionKey değerini bize döndüren metot.
		private string GetCartId(HttpContext context)
		{
            if (context.Session.GetString("CartSessionKey") == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    //kişi login durumda ise bu durumda setString ile Session değer atayalım.
                    context.Session.SetString("CartSessionKey", context.User.Identity.Name);
                }
                else
                {
                    //eğer kişi login durumda değilse  

					context.Session.SetString("CartSessionKey",  Guid.NewGuid().ToString());
				}
			}

            return context.Session.GetString("CartSessionKey");
		}

        //sepete  ürün ekleme
        //kişi daha önce eklemek istediği ürünü sepetine eklemişse ,adet bilgisini artıran, hiç eklememişse 0dan o anki ürünün bilgilerini cart tablosuna kaydeden metot.
        public void AddToCart(Album album)
        {
            //Album album=_context.Albums.Where(satir => satir.AlbumId == album.AlbumId).FirstOrDefault();
            //sepette o kişi tarafından daha önce eklenmiş bir album var mı?
            var cartItem=_context.Carts.FirstOrDefault(satir => satir.CartId == ShoppingCartId && satir.AlbumId == album.AlbumId);

            if(cartItem == null)
            {
                //siparisi 0dan oluşturman lazım
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1, //ilk ürün eklenirken adet bilgisi 1 olmalıdır
                    DateCreated = DateTime.Now
                };

                _context.Carts.Add(cartItem);
                    
            }
            else
            {
                //adet artacak
                cartItem.Count++; //daha önce o kişi o albumu eklemişse, o kayıttaki adet bilgisi 1 artırılacak
            }

            _context.SaveChanges();
        }


		//sepetten ürün silme işlemi gerçekleşir, silme sonra o üründen kaç tane kaldıgının bilgisini bize geriye döndürür.        
        public int RemoveFromCart(int id)
        {
            var cartItem=_context.Carts.FirstOrDefault(satir => satir.CartId == ShoppingCartId && satir.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1 )//son ürün kalana kadar adet bilgisi düşürülür, 1 tane kalan ürün tekrar sepetten silinirse 0 yazmak yerine komple ürün silinir.
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _context.Carts.Remove(cartItem);
                }
                _context.SaveChanges();
            }
            return itemCount;

        }


		//sepeti boşaltma
		//sepetteki ürünler Carts tablosunda tutulur. o an alışveriş yapan kişinin sepetindeki ürünleri komple silen metot.
		public void EmptyCart()
        {            
            var cartItems=_context.Carts.Where(satir => satir.CartId == ShoppingCartId);
            foreach (var item in cartItems)
            {
                _context.Carts.Remove(item);
            }
            _context.SaveChanges();
        }


		//sepetteki ürün adedini çekme,
        //o an alışveriş yapan kişinin sepetindeki ürün adedini çeker.
        public int GetCount()
        {
            int? adet = _context.Carts.Where(satir => satir.CartId == ShoppingCartId).Select(satir => satir.Count).Sum();
            if (adet == null)
                return 0;
            else
                return adet.Value;
        }

        //sepetteki ürünlerin album bilgisini getirecek olan GetCartItems metodunu yazalım.

        public List<Cart> GetCartItems()
        {
            return _context.Carts.Include(satir=>satir.Album).Where(p=>p.CartId== ShoppingCartId).ToList();
        }

        public ShoppingCart GetCartItems(HttpContext context)
        {
            return GetCart(context);
        }

        //sepetteki ürünlerin fiyatlarının toplamını bulan metot.
        public decimal GetTotal()
        {
            decimal result=0;
            var cartItems = GetCartItems(); //sepete eklenen ürünleri çektim. bu ürünlerde tek tek dolanıp, güncel fiyatını ve adet bilgisini bulup. bu 2 bilgiyi çarpıp toplam tutar (result) değişkenine ekledik.
            int adet;
            decimal listprice;
			foreach (var item in cartItems)
            {
                adet=item.Count;
                listprice = item.Album.Fiyat;
                result+=adet* listprice;
            }
            return result;
        }


        //sipariş oluştur metodu.
        //order tablosuna sipariş bilgileriini girerek yeni bir kayıt eklesin, bu ekleme sonrası elde ettiğimi orderid bilgisini (siparis id degeri) bize döndüren bir metot.
        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;

            var cartItems=GetCartItems();

            foreach (var item in cartItems)
            {
                //1.order detail tarafına bu sepetteki ürünler eklenecek.
                //2.toplam siparisş tutarı hesaplanıp o bilgi orders tablosuna eklenecek.
                OrderDetail orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Fiyat,
                    Quantity = item.Count
                };

                orderTotal += item.Count + item.Album.Fiyat;

                _context.OrderDetails.Add(orderDetail);                
            }

            order.Total = orderTotal;

            _context.SaveChanges();

            //todo:
            //sepeti boşaltma ihtiyacıı...
            return order.OrderId;
        }


		//login olmayan kişinin yaptıgı işlemleri login olunca otomatik o kişiye atama (migrate)
        public void MigrateCart(string userName)
        {
            var shoppingCart=_context.Carts.Where(satir => satir.CartId == ShoppingCartId);

            foreach (var item in shoppingCart)
            {
                item.CartId = userName;
            }
            _context.SaveChanges();
        }
        

	}
}
