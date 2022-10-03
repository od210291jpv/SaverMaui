namespace SaverBackend.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Guid CategoryId { get; set; }

        public int? AmountOfOpenings { get; set; }

        public int? AmountOfFavorites { get; set; }
    }
}
