using ImageRecognitionNN;
using Microsoft.AspNetCore.Mvc;

namespace BananasGamblerBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetCardCategory : ControllerBase
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
