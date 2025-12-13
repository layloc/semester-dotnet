using Microsoft.EntityFrameworkCore;
using SEM.Domain.Abstractions;
using SEM.Entities;

namespace SEM.Infrastructure.Repositories;

public class UserRepository :  IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await _db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email) ?? throw new Exception();
    }
}