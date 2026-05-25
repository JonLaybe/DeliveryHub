using Auth.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configurations;

public sealed class ServiceClientConfiguration : IEntityTypeConfiguration<ServiceClientEntity>
{
    public void Configure(EntityTypeBuilder<ServiceClientEntity> b)
    {
        b.ToTable("service_clients");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
        b.Property(x => x.SecretHash).HasColumnName("secret_hash").IsRequired();
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.AllowedScopes).HasColumnName("allowed_scopes").IsRequired();

        b.HasIndex(x => x.ClientId).IsUnique();
    }
}