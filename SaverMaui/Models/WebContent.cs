using Realms;

namespace SaverMaui.Models
{
    public class WebContent : RealmObject
    {
        public Guid Id { get; set; }

        public string Source { get; set; }

        public string Title { get; set; }
    }
}
