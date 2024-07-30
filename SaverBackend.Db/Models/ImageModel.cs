namespace SaverBackend.Models
{
    public class ImageModel
    {
        public int Id { get; set; }

        public string FileName { get; set; } = string.Empty;

        public byte[] Content { get; set; } = Array.Empty<byte>();
    }
}
