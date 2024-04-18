using AChat.Domain;
using AChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Message = AChat.Domain.Entities.Message;

namespace AChat.Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.Property(m => m.Content)
            .IsRequired();

        builder.HasOne(m => m.Contact)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ContactId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.OwnsMany(m => m.Attachments,
            b => b.ToJson());
    }
}
