using SEM.Entities;

namespace SEM.Domain.Abstractions;

public interface IUserRepository
{
    public Task<User?> GetUserByEmail(string email);
    public Task AddAsync(User user);
}