using Flurl.Http;
using RateMLModelConsole;
using Tensorflow;

namespace RateRecognitionNN
{
    public class ImageRateAnalyzer
    {
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        public async Task<RateMLModel.ModelOutput> AnalyzeImageByUrl(string imageUri)
        {
            var t = imageUri.DownloadFileAsync($"{this.directory}/data/");
            await Task.WhenAll(t);

            //await imageUri.DownloadFileAsync($"{this.directory}/data/");

            var file = Directory.GetFiles($"{this.directory}/data/");
            var result = this.AnalyzeImage(Path.GetFullPath(file.First()));
            File.Delete(Path.GetFullPath(file.First()));
            return result;
        }

        public RateMLModel.ModelOutput AnalyzeImage(string imagePath)
        {
            if (File.Exists(imagePath)) 
            {
                var imageBytes = File.ReadAllBytes(imagePath);
                RateMLModel.ModelInput sampleData = new RateMLModel.ModelInput()
                {
                    ImageSource = imageBytes,
                };

                RateMLModel.ModelOutput result = RateMLModel.Predict(sampleData);
                return result;
            }

            return default;
        }
    }
}
