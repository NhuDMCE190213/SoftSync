using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftSync.DAL.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(Data.SoftSyncDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string email)
            => await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        public async Task<bool> EmailExistsAsync(string email)
            => await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
}
