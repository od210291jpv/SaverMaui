namespace BananasGamblerBackend.Database.SaverBackendModels
{
    public class Group
    {
        public int Id { get; set; }

        public Guid GroupId { get; set; }

        public string Name { get; set; }

        public List<Category> GrouppedCategories { get; set; }
    }
}
