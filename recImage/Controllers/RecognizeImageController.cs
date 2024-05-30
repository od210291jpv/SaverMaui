using ImageRecognitionNN;
using Microsoft.AspNetCore.Mvc;

namespace recImage.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecognizeImageController : Controller
    {
        [HttpGet]
        public async Task<string> Index(string imageUri)
        {

            ImageAnalyzer nn = new ImageAnalyzer();
            var category = await nn.AnalyzeImageByUrl(imageUri);
            return category.First().Key;
        }
    }
}
