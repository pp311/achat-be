using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AChat.Domain;
using AChat.Domain.Entities;

namespace AChat.Infrastructure.Configurations;

public class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(StringLength.Name);

        builder.Property(t => t.Content)
            .IsRequired();

        builder.Property(s => s.Type)
            .HasConversion(v => v.ToString(),
                v => (TemplateType)Enum.Parse(typeof(TemplateType), v));
    }
}
