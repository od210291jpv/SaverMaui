using System.ComponentModel.DataAnnotations;

namespace BananasGamblerBackend.Dto
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Login was not specified")]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password was not specified")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
