using Newtonsoft.Json;
using SaverMaui.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.Services.Contracts
{
    public class GetAllCategoriesResponseModel : IResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("categoryId")]
        public Guid CategoryId { get; set; }
    }
}
