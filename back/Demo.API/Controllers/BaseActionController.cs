namespace Demo.API.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using Demo.API.Models;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class BaseActionController : Controller
    {
        protected readonly UserManager<ApplicationUser> _userManager;

        public BaseActionController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<ApplicationUser> GetDefaultUser()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this._userManager.FindByIdAsync(userId);
            return user;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetDefaultUserAccount()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this._userManager.FindByIdAsync(userId);
            return Convert.ToString(user.AccountId);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetDefaultUserId()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this._userManager.FindByIdAsync(userId);
            return Convert.ToString(user.Id);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetDefaultUserName()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this._userManager.FindByIdAsync(userId);
            return Convert.ToString(user.FirstName + " " + user.LastName);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<string> GetUserNameOrEmail()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await this._userManager.FindByIdAsync(userId);
            return string.IsNullOrWhiteSpace(Convert.ToString(user.FirstName + " " + user.LastName))
                       ? user.Email
                       : Convert.ToString(user.FirstName + " " + user.LastName);
        }
    }
}