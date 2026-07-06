using Microsoft.EntityFrameworkCore;
using SoftSync.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftSync.DAL.Repositories
{
    public interface IPasswordResetTokenRepository : IRepository<PasswordResetToken>
    {
        Task<PasswordResetToken?> GetValidTokenAsync(string token);
    }

    public class PasswordResetTokenRepository : Repository<PasswordResetToken>, IPasswordResetTokenRepository
    {
        public PasswordResetTokenRepository(Data.SoftSyncDbContext context) : base(context) { }

        public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
        {
            return await _dbSet
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsUsed && t.ExpiresAt > DateTime.UtcNow);
        }
    }
}
