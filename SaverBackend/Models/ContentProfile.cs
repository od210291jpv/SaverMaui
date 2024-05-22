using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    [Table("contentprofile1")]
    public class ContentProfile
    {
        [Key]
        public int FavoriteContentId { get; set; }

        public int ProfileId { get; set; }
    }
}
