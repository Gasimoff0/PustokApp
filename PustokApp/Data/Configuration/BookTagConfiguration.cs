using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PustokApp.Models;

namespace PustokApp.Data.Configuration
{
    public class BookTagConfiguration : IEntityTypeConfiguration<BookTag>
    {
        public void Configure(EntityTypeBuilder<BookTag> builder)
        {
            builder.HasKey(bt => new { bt.BookId, bt.TagId });
        }
    }
}
