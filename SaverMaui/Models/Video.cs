using Realms;

namespace SaverMaui.Models
{
    public class Video : RealmObject
    {
        public string Title { get; set; }

        public string VideoUri { get; set; }

        public Guid? CategoryId { get; set; }

        public bool IsFavorite { get; set; }
    }
}
