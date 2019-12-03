using AnkaCMS.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnkaCMS.Data.DataAccess.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný ContentLanguageLine tablosu konfigürasyonu
    /// </summary>
    internal class ContentLanguageLineConfiguration : IEntityTypeConfiguration<ContentLanguageLine>
    {
        public void Configure(EntityTypeBuilder<ContentLanguageLine> builder)
        {
            builder.ToTable("ContentLanguageLines");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.HasIndex(x => x.DisplayOrder).IsUnique(false).HasName("IX_ContentLanguageLineDisplayOrder");
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.HasOne(x => x.Creator).WithMany(y => y.ContentLanguageLinesCreatedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.LastModificationTime).IsRequired();
            builder.HasOne(x => x.LastModifier).WithMany(y => y.ContentLanguageLinesLastModifiedBy).IsRequired().OnDelete(DeleteBehavior.Restrict);

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(400)");
            builder.HasIndex(x => x.Code).IsUnique().HasName("UK_ContentLanguageLineCode");
            builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(4000)");
            builder.Property(x => x.ShortName).HasColumnType("varchar(400)");
            builder.Property(x => x.Description).HasColumnType("varchar(4000)");
            builder.Property(x => x.Keywords).HasColumnType("varchar(4000)");
            builder.Property(x => x.Url).HasColumnType("varchar(4000)");
            builder.Property(x => x.ContentDetail).HasColumnType("text");
            builder.Property(x => x.ImageName).HasColumnType("varchar(400)");
            builder.Property(x => x.ImagePath).HasColumnType("varchar(4000)");
            builder.Property(x => x.ImageFileType).HasColumnType("varchar(400)");
            builder.Property(x => x.ImageFileLength);
            builder.Property(x => x.ViewCount).IsRequired();

            builder.HasOne(x => x.Content).WithMany(y => y.ContentLanguageLines).IsRequired().OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Language).WithMany(y => y.ContentLanguageLines).IsRequired().OnDelete(DeleteBehavior.Restrict);


        }
    }
}
