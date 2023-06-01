using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.Contracts.Video
{
    public class SyncVideoRequestDto : IRequest
    {
        public Guid PublisherId { get; set; }

        public AddVideoRequestDto[] VideoContent { get; set; } = Array.Empty<AddVideoRequestDto>();
    }
}
