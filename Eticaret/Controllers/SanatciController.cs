using Eticaret.Data;
using Eticaret.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eticaret.Controllers
{
    public class SanatciController : Controller
    {
        private readonly EticaretVeritabaniContext ctx;

        public SanatciController(EticaretVeritabaniContext eticaretVeritabaniContext)
        {
            ctx = eticaretVeritabaniContext;  // new ile örneklemiş gibi context içini doldurduk
        }

        public IActionResult Index()
        {
            //Artist tablosuna git ve dataları getir.
            //1.contexti class içerisnde tanımlamak
            List<Artist> list= ctx.Artists.ToList();
            return View(list); //list verisinin viewde yani sayfada kullanılmasını sağladık.
        }
    }
}
