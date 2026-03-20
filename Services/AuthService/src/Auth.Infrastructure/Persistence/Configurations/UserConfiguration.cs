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

        b.Property(x => x.NormalizedEmail)
            .HasColumnName("normalized_email")
            .HasMaxLength(320)
            .IsRequired();
        
        b.Property(x => x.UserName)
            .HasColumnName("user_name") // email будем использовать как username
            .HasMaxLength(320)
            .IsRequired();

        b.Property(x => x.NormalizedUserName)
            .HasColumnName("normalized_user_name")
            .HasMaxLength(320)
            .IsRequired();

        b.Property(x => x.PhoneNumber)
            .HasColumnName("phone")
            .HasMaxLength(32);

        b.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        b.Property(x => x.Status)
            .HasColumnName("status")
            .HasConversion<short>()
            .IsRequired();

        b.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValueSql("now()");
        
        b.Property(x => x.UpdatedAt)
            .HasColumnName("updated_at")
            .HasDefaultValueSql("now()");

        b.Property(x => x.LockoutEnd)
            .HasColumnName("lockout_end");
        
        b.Property(x => x.AccessFailedCount)
            .HasColumnName("failed_login_count")
            .HasDefaultValue(0);

        b.Property(x => x.EmailConfirmed)
            .HasColumnName("email_confirmed")
            .HasDefaultValue(false);

        b.Property(x => x.PhoneNumberConfirmed)
            .HasColumnName("phone_number_confirmed")
            .HasDefaultValue(false);
        
        b.Property(x => x.TwoFactorEnabled)
            .HasColumnName("two_factor_enabled")
            .HasDefaultValue(false);
        
        b.Property(x => x.LockoutEnabled)
            .HasColumnName("lockout_enabled")
            .HasDefaultValue(true);

        b.Property(x => x.SecurityStamp)
            .HasColumnName("security_stamp")
            .IsRequired();

        b.Property(x => x.ConcurrencyStamp)
            .HasColumnName("concurrency_stamp")
            .IsRequired();

        b.HasIndex(x => x.Email).IsUnique()
            .HasDatabaseName("IX_users_email");

        b.HasIndex(x => x.NormalizedUserName)
            .IsUnique()
            .HasDatabaseName("IX_users_normalized_user_name");
        
        b.HasIndex(x => x.NormalizedEmail)
            .HasDatabaseName("IX_users_normalized_email");
    }
}