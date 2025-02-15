using Flurl.Http;
using RateMLModelConsole;

namespace RateRecognitionNN
{
    public class ImageRateAnalyzer
    {
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        public async Task<RateMLModel.ModelOutput> AnalyzeImageByUrl(string imageUri)
        {
            await imageUri.DownloadFileAsync($"{this.directory}/data/");

            var file = Directory.GetFiles($"{this.directory}/data/");
            var result = this.AnalyzeImage(Path.GetFullPath(file.First()));
            File.Delete(file.First());
            return result;
        }

        public RateMLModel.ModelOutput AnalyzeImage(string imagePath)
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            RateMLModel.ModelInput sampleData = new RateMLModel.ModelInput()
            {
                ImageSource = imageBytes,
            };

            RateMLModel.ModelOutput result = RateMLModel.Predict(sampleData);
            return result;
        }
    }
}
