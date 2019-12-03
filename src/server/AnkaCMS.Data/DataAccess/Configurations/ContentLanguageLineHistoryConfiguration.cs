using AnkaCMS.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnkaCMS.Data.DataAccess.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabaný PersonHistory tablosu konfigürasyonu
    /// </summary>
    internal class ContentLanguageLineHistoryConfiguration : IEntityTypeConfiguration<ContentLanguageLineHistory>
    {
        public void Configure(EntityTypeBuilder<ContentLanguageLineHistory> builder)
        {
            builder.ToTable("ContentLanguageLineHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();

            builder.Property(x => x.Code).IsRequired().HasColumnType("varchar(400)");
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
            builder.Property(x => x.ContentId).IsRequired();

            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.LanguageId).IsRequired();

        }
    }
}
