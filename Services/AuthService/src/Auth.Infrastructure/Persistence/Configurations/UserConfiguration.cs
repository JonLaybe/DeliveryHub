using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Auth.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("gen_random_uuid()");

        b.Property(x => x.Email)
            .HasColumnName("email")
            .IsRequired()
            .HasMaxLength(320);

        b.HasIndex(x => x.Email).IsUnique();

        b.Property(x => x.Phone)
            .HasColumnName("phone")
            .HasMaxLength(32);

        b.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()")
            .IsRequired();

        b.Property(x => x.LockoutEnd)
            .HasColumnName("lockout_end");

        b.Property(x => x.FailedLoginCount)
            .HasColumnName("failed_login_count")
            .HasDefaultValue(0)
            .IsRequired();
    }
}