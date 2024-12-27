using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentSorter
{
    public class PostContentDataRequest
    {
        [JsonProperty("categories")]
        public CategoryDto[] Categories { get; set; }

        [JsonProperty("content")]
        public ContentDto[] Content { get; set; }
    }

    public class CategoryDto 
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }

        [JsonProperty("amountOfOpenings")]
        public int? AmountOfOpenings { get; set; }

        [JsonProperty("amountOfFavorites")]
        public int? AmountOfFavorites { get; set; }

        [JsonProperty("PublisherProfileId")]
        public Guid? PublisherProfileId { get; set; }
    }

    public class ContentDto 
    {
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("imageUri")]
        public string ImageUri { get; set; }

        [JsonProperty("categoryId")]
        public Guid? CategoryId { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }
    }
}
