namespace SaverBackend.Models
{
    public class ImageModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }
    }
}
