namespace LiveHost.DataBase.Models
{
    public class Group
    {
        public int Id { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; } = string.Empty;

        public List<Category> GrouppedCategories { get; set; } = new List<Category>();
    }
}