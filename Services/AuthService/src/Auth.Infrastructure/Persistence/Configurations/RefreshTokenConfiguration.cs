using Auth.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configurations;

public sealed class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    public void Configure(EntityTypeBuilder<RefreshTokenEntity> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Id).HasColumnName("id").HasDefaultValueSql("gen_random_uuid()");
        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();

        b.Property(x => x.TokenHash).HasColumnName("token_hash").IsRequired();
        b.Property(x => x.ExpiresAt).HasColumnName("expires_at").IsRequired();

        b.Property(x => x.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("now()");
        b.Property(x => x.RevokedAt).HasColumnName("revoked_at");
        b.Property(x => x.ReplacedByHash).HasColumnName("replaced_by_hash");

        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.UserId);

        b.HasOne<Auth.Domain.Entities.User>()
            .WithMany()
            .HasForeignKey(x => x.UserId)// здесь ссылочный ключ на user.id
            .OnDelete(DeleteBehavior.Cascade);
    }
}