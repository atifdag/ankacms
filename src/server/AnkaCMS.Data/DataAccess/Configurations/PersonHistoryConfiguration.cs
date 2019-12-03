using AnkaCMS.Data.DataEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AnkaCMS.Data.DataAccess.Configurations
{

    /// <inheritdoc />
    /// <summary>
    /// Veri tabanı PersonHistory tablosu konfigürasyonu
    /// </summary>
    internal class PersonHistoryConfiguration : IEntityTypeConfiguration<PersonHistory>
    {
        public void Configure(EntityTypeBuilder<PersonHistory> builder)
        {
            builder.ToTable("PersonHistories");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).IsRequired();
            builder.Property(x => x.DisplayOrder).IsRequired();
            builder.Property(x => x.IsApproved).IsRequired();
            builder.Property(x => x.Version).IsRequired();
            builder.Property(x => x.CreationTime).IsRequired();
            builder.Property(x => x.CreatorId).IsRequired();
            builder.Property(x => x.ReferenceId).IsRequired();
            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.RestoreVersion).IsRequired();


            builder.Property(x => x.FirstName).IsRequired().HasColumnType("varchar(100)");
            builder.Property(x => x.LastName).IsRequired().HasColumnType("varchar(100)");
            builder.Property(x => x.IdentityCode).HasColumnType("varchar(36)");
            builder.Property(x => x.BirthDate).IsRequired();
            builder.Property(x => x.Biography).HasColumnType("text");

        }
    }
}
