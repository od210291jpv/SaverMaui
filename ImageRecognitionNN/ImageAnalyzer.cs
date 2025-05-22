using Flurl.Http;
using ImageRecognitionNN.Enums;

namespace ImageRecognitionNN
{
    public class ImageAnalyzer
    {
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


        public async Task<ImageLearning.ModelOutput> AnalyzeImageByUrl(string imageUri) 
        {
            await imageUri.DownloadFileAsync($"{this.directory}/data/");

            var file = Directory.GetFiles($"{this.directory}/data/");
            var result = this.AnalyzeImage(Path.GetFullPath(file.First()));
            File.Delete(file.First());
            return result;
        }

        public ImageLearning.ModelOutput AnalyzeImage(string imagePath) 
        {
            var imageBytes = File.ReadAllBytes(imagePath);
            ImageLearning.ModelInput sampleData = new ImageLearning.ModelInput()
            {
                ImageSource = imageBytes,
            };

            ImageLearning.ModelOutput result =  ImageLearning.Predict(sampleData);
            return result;
        }

        public void GetImagesByLabel(Labels label, string derectoryPath) 
        {
            var outputDir = $"{Directory.GetCurrentDirectory()}/{label}/Output";
            Directory.CreateDirectory(outputDir);
            var allFiles = Directory.GetFiles(derectoryPath);

            int counter = 0;
            int allFilestCount = allFiles.Length;

            foreach (var item in allFiles)
            {
                counter++;

                Console.WriteLine($"Processing: {counter}/{allFilestCount} file...");
                var labels = this.AnalyzeImage(item);
            }
        }

    }
}