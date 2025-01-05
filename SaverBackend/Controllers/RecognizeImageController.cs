using ImageRecognitionNN;
using Microsoft.AspNetCore.Mvc;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecognizeImageController : Controller
    {
        [HttpGet]
        public async Task<string> Index(string imageUri)
        {
            ImageAnalyzer nn = new ImageAnalyzer();
            ImageLearning.ModelOutput category = await nn.AnalyzeImageByUrl(imageUri);
            return category.PredictedLabel;
        }
    }
}
