namespace SaverBackend.DTO
{
    public class SyncVideoDto
    {
        public Guid PublisherId { get; set; }

        public AddVideoDto[] VideoContent { get; set; } = Array.Empty<AddVideoDto>();
    }
}
