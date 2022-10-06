namespace SaverMaui.Services.Contracts.Profile
{
    internal class CreateProfileRequestDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VerificationCode { get; set; }
    }
}
