using Microsoft.EntityFrameworkCore;

namespace LiveHost.DataBase
{
    public class ApplicationContext : DbContext
    {
        public static string ConnectionString = string.Empty;
    }
}
