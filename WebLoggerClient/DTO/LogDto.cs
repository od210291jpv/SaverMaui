using System.ComponentModel.DataAnnotations;

namespace WebLoggerClient.DTO
{
    public class LogDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public LogSeverity Severity { get; set; }
    }
}
