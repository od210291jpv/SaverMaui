using ImageRecognitionNN;
using Microsoft.AspNetCore.Mvc;
using RateRecognitionNN;
using SaverBackend.Services.RabbitMq;
using System.Net;

namespace SaverBackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecognizeImageController : Controller
    {
        private IRabbitMqService mqService;
        //private LoggerClient webLogger = new LoggerClient("http://192.168.88.68:8081");

        public RecognizeImageController(IRabbitMqService mqService)
        {
            this.mqService = mqService;
        }

        [HttpGet]
        public async Task<string> Index(string imageUri)
        {
            //await this.webLogger.LogAsync($"Starting image analysis for URL: {imageUri}", LogSeverity.Verbose);
            ImageAnalyzer nn = new ImageAnalyzer();
            //await this.webLogger.LogAsync("ImageAnalyzer instance created", LogSeverity.Verbose);
            ImageLearning.ModelOutput category = await nn.AnalyzeImageByUrl(imageUri);
            //await this.webLogger.LogAsync($"Image analysis completed. Predicted category: {category.PredictedLabel}", LogSeverity.Verbose);
            return category.PredictedLabel;
        }

        [HttpGet("FilterByCategory")]
        public async Task<HttpStatusCode> FilterFoundImagesByCategory(string category) 
        {
            //await this.webLogger.LogAsync($"Sending category '{category}' to FilterContentQueue", LogSeverity.Verbose);
            this.mqService.SendMessage(category, "FilterContentQueue");
            //await this.webLogger.LogAsync($"Category '{category}' sent to FilterContentQueue successfully", LogSeverity.Verbose);
            return HttpStatusCode.OK;
        }

        [HttpGet("RecognizeImageRate")]
        public async Task<string> RecognizeCategory(string imageUri) 
        {
            //await this.webLogger.LogAsync($"Starting image rate analysis for URL: {imageUri}", LogSeverity.Verbose);
            ImageRateAnalyzer nn = new ImageRateAnalyzer();
            //await this.webLogger.LogAsync("ImageRateAnalyzer instance created", LogSeverity.Verbose);
            var rate = await nn.AnalyzeImageByUrl(imageUri);
            //await this.webLogger.LogAsync($"Image rate analysis completed. Predicted rate: {rate.PredictedLabel}", LogSeverity.Verbose);
            return rate.PredictedLabel;
        }
    }
}
