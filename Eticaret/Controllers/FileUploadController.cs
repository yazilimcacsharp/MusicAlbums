using Microsoft.AspNetCore.Mvc;

namespace Eticaret.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        //IWebHostEnvironment

        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileUploadController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }



        //metot yazılacak. gelen fotografı proje içine dosya olarak kaydetmesi için


        [HttpPost("uploadFile2")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            //bu durumda yükleme işlemi yapılacak
            //1.uploadsFile bilgisi oluşturulacak.
            //2.filename bilgisinde bir Guid yapısı kullanmak gerekebilir.

            //bu fotografı kim yükledi görmek istenirse diye, fotografın adına kullanıcı adını, mail adresini, o kişiye özel unique bi değeri dosya adına eklemek güzel olur.
            var fileName = file.FileName+";"+User.Identity.Name +";"+ Path.GetExtension(file.FileName);
          
            string uploadPath = GetUploadPath(); //dosyanın yüklendiği dosya yolunu alacak.

            if (file != null && file.Length > 0)
            {                
                try
                {
                    var filePath = Path.Combine(uploadPath, fileName); //uploads/dosyaAdi
                    FileStream fs = new FileStream(filePath, FileMode.Create); 
                    await file.CopyToAsync(fs);
                }
                catch (Exception ex)
                {
                    string hata = ex.Message;
                }

            }

            return RedirectToAction("Albumler", "Album");
        }

        private string GetUploadPath()
        {
            //Create uploads: folderını oluşturup yüklendiği dosya yolunu
            //alacağız
            var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            return uploadPath;

        }
    }
}
