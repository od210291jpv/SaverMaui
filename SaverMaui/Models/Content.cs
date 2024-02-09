using Realms;

namespace SaverMaui.Models
{
    public class Content : RealmObject
    {
        public string Title { get; set; }

        public string ImageUri { get; set; }

        public Guid? CategoryId { get; set; }

        public bool IsFavorite { get; set; }

        public int Rating { get; set; } = 0;
    }
}
