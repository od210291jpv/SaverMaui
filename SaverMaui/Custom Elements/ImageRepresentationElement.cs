using Realms;

using SaverMaui.Models;
using SaverMaui.Services;
using System.Net;

namespace SaverMaui.Custom_Elements
{
    public class ImageRepresentationElement : Image, ICustomElement
    {
        public int ContentId { get; set; }

        public HttpStatusCode StatusCode 
        {
            get 
            {
                return BackendServiceClient.GetInstance().GetSourceStatusCode(new Uri(this.Source.ToString())).Result;
            }
        }

        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName 
        {
            get 
            {
                return Realm.GetInstance().All<Category>().Single(c => CategoryId == this.CategoryId).Name;
            }
            set => CategoryName = value;
        }

        public bool IsFavorite { get; set; }

        public int Rating { get; set; } = 0;
    }
}
