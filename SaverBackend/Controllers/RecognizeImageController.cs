using ImageRecognitionNN;
using Microsoft.AspNetCore.Mvc;
using SaverBackend.Services.RabbitMq;
using System.Net;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecognizeImageController : Controller
    {
        private IRabbitMqService mqService;

        public RecognizeImageController(IRabbitMqService mqService)
        {
            this.mqService = mqService;
        }

        [HttpGet]
        public async Task<string> Index(string imageUri)
        {
            ImageAnalyzer nn = new ImageAnalyzer();
            ImageLearning.ModelOutput category = await nn.AnalyzeImageByUrl(imageUri);
            return category.PredictedLabel;
        }

        [HttpGet("FilterByCategory")]
        public HttpStatusCode FilterFoundImagesByCategory(string category) 
        {
            this.mqService.SendMessage(category, "FilterContentQueue");
            return HttpStatusCode.OK;
        }
    }
}
