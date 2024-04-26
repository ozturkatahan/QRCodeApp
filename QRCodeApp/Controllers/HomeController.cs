using Microsoft.AspNetCore.Mvc;
using QRCodeApp.Models;
using System.Diagnostics;
using System.Drawing;
using ZXing;
using ZXing.QrCode;

namespace QRCodeApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(IFormCollection formCollection) 
        {
            var writer = new QRCodeWriter();
            var resultBit = writer.encode(formCollection["QRCodeString"], BarcodeFormat.QR_CODE,200,200);
            var matrix = resultBit;
            int scale = 2;
            Bitmap result = new Bitmap(matrix.Width * scale, matrix.Height * scale);
            for (int i = 0; i<matrix.Height; i++)
            {
                for (int j = 0; j< matrix.Width; j++)
                {
                    Color pixel = matrix[i, j] ? Color.Black : Color.White;
                    for (int k = 0; k < scale; k++)
                        for (int l = 0; l < scale; l++)
                            result.SetPixel(i * scale + k, j * scale + l, pixel);
                }
            }
            string webRootPath = _webHostEnvironment.WebRootPath;
            result.Save(webRootPath+"\\Images\\QRCodeNew.png");
            ViewBag.URL = "\\Images\\QRCodeNew.png";
            return View();
        }

        public IActionResult ReadQRCode()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var path = webRootPath + "\\Images\\QRCodeNew.png";
            var reader = new BarcodeReaderGeneric();
            Bitmap image = (Bitmap)Image.FromFile(path);
            using (image)
            {
                LuminanceSource source = new ZXing.Windows.Compatibility.BitmapLuminanceSource(image);
                Result result = reader.Decode(source);
                ViewBag.Text = result.Text;
            }
            return View("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
