using SaverMaui.Services.Interfaces;

namespace SaverMaui.Services.Contracts.Profile
{
    internal class CreateProfileRequestDto : IRequest
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VerificationCode { get; set; }
    }
}
