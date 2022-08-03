using Realms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaverMaui.Models
{
    public class Content : RealmObject
    {
        public string Title { get; set; }

        public string ImageUri { get; set; }

        public Guid? CategoryId { get; set; }

        public bool IsFavorite { get; set; }
    }
}
