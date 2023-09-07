using System.ComponentModel.DataAnnotations;

namespace BananasGamblerBackend.Dto
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Login not set")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password not set")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
