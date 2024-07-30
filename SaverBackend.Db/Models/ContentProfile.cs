using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaverBackend.Models
{
    [Table("contentprofile")]
    public class ContentProfile
    {
        //[Key]
        public int FavoriteContentId { get; set; }

        public int ProfileId { get; set; }
    }
}
