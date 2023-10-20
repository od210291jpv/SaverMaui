namespace BananasGambler.DTO
{
    internal class CategoryDto
    {
        public long Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
