using System.ComponentModel.DataAnnotations;

namespace ContentParserBackend.Dto
{
    public class LogDto
    {
        [Required]
        public string Message { get; set; } = string.Empty;

        [Required]
        public LogSeverity Severity { get; set; }
    }
}
