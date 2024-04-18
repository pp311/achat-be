using AChat.Domain;
using AChat.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AChat.Infrastructure.Configurations;

public class SourceConfiguration : IEntityTypeConfiguration<Source>
{
    public void Configure(EntityTypeBuilder<Source> builder)
    {
        builder.Property(s => s.Type)
            .HasConversion(v => v.ToString(),
                v => (SourceType)Enum.Parse(typeof(SourceType), v));
    }
}