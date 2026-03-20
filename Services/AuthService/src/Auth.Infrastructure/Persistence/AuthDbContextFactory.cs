using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Auth.Infrastructure.Persistence;

public sealed class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var cs =
            Environment.GetEnvironmentVariable("ConnectionStrings__Postgres")
            ?? "Host=localhost;Port=5432;Database=auth_db;Username=auth_user;Password=auth_pwd";

        var options = new DbContextOptionsBuilder<AuthDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new AuthDbContext(options);
    }
}