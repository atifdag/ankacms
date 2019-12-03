using AnkaCMS.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnkaCMS.Data.DataAccess.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný PartContentLine tablosu konfigürasyonu
    /// </summary>
    internal class PartContentLineConfiguration : IEntityTypeConfiguration<PartContentLine>
    {
        public void Configure(EntityTypeBuilder<PartContentLine> builder)
        {
            builder.ToTable("PartContentLines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_PartContentLineDisplayOrder");
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.PartContentLinesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.PartContentLinesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(x => x.Part).WithMany(y => y.PartContentLines).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Content).WithMany(y => y.PartContentLines).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}
