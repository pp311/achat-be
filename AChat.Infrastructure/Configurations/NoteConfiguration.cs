using AChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AChat.Infrastructure.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.Property(n => n.Content)
            .IsRequired();

        builder.HasOne(n => n.Contact)
            .WithMany(c => c.Notes)
            .HasForeignKey(n => n.ContactId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}