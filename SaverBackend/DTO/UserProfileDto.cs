using SaverBackend.Models;

namespace SaverBackend.DTO
{
    public class UserProfileDto
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string VerificationCode { get; set; }
    }
}
