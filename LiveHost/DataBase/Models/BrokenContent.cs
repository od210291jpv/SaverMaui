using System.ComponentModel.DataAnnotations.Schema;

namespace LiveHost.DataBase.Models
{
    public class BrokenContent
    {
        [ForeignKey(nameof(Id))]
        public int Id { get; set; }

        public string Title { get; set; }

        public string ImageUri { get; set; }

        public Guid? CategoryId { get; set; }
    }
}
