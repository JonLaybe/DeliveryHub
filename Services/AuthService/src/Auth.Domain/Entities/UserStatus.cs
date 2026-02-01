namespace Auth.Domain.Entities;

public enum UserStatus : short
{
    Active = 0,
    Locked = 1,
    Deleted = 2
}