using Demo.API.Models;

namespace Demo.API.Services
{
    using System.Threading.Tasks;

    using Demo.API.Data;
    using Demo.API.Interfaces;

    using Microsoft.EntityFrameworkCore;

    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            this._context = context;
        }

        public async Task<bool> UpdateUserStatus(string userId, bool isDisable)
        {
            try
            {
                var users = await this._context.Users.FirstOrDefaultAsync(x => x.Id == userId);
                users.Disabled = isDisable;
                await this._context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}