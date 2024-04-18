using AChat.Domain;
using AChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AChat.Infrastructure.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(StringLength.Name);

        builder.HasMany(t => t.Contacts)
            .WithMany(c => c.Tags)
            .UsingEntity<ContactTag>(
                x => x.HasOne(ct => ct.Contact)
                    .WithMany()
                    .HasForeignKey(ct => ct.ContactId),
                x => 
                    x.HasOne(ct => ct.Tag)
                    .WithMany()
                    .HasForeignKey(ct => ct.TagId));
    }
}