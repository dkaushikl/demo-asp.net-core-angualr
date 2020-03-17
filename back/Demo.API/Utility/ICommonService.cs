namespace Demo.API.Utility
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;

    public interface ICommonService
    {
        string GenerateRandomPassword(PasswordOptions opts = null);

        Task<string> GenerateToken(string email, string password);
    }
}