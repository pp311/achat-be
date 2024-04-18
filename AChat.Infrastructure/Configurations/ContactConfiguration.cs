using AChat.Domain;
using AChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AChat.Infrastructure.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(StringLength.Name);

        builder.Property(c => c.PhoneNumber)
            .HasMaxLength(StringLength.Phone);

        builder.Property(c => c.Email)
            .HasMaxLength(StringLength.Email);

        builder.Property(c => c.Gender)
            .HasConversion(v => v.ToString(),
                v => (Gender)Enum.Parse(typeof(Gender), v));

        builder.Property(c => c.AvatarUrl)
            .HasMaxLength(StringLength.Url);
        
        builder.HasOne(c => c.Source)
            .WithMany()
            .HasForeignKey(c => c.SourceId)
            .OnDelete(DeleteBehavior.SetNull);
        
    }
}