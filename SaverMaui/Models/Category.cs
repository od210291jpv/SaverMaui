using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.Models
{
    public class Category : RealmObject
    {
        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        public bool IsFavorite { get; set; }

        public int AmountOfOpenings { get; set; } = 0;

        public int AmountOfFavorites { get; set; } = 0;
    }
}
