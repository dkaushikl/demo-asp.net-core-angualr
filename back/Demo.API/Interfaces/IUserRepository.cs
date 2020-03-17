using Demo.API.Models;

namespace Demo.API.Interfaces
{
    using System.Threading.Tasks;

    public interface IUserRepository
    {
        Task<bool> UpdateUserStatus(string userId, bool isDisable);
    }
}