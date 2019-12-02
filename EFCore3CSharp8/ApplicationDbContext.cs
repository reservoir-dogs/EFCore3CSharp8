using Microsoft.EntityFrameworkCore;

namespace EFCore3CSharp8
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        public DbSet<Foo> Foos { get; set; } = null!;
    }
}
