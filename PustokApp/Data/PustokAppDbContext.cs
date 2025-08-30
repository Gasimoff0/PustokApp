using Microsoft.EntityFrameworkCore;
using PustokApp.Models;

namespace PustokApp.Data
{
    public class PustokAppDbContext(DbContextOptions<PustokAppDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PustokAppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<Setting> Settings { get; set; }
    }
}
