namespace Demo.API.Models
{
    using Microsoft.AspNetCore.Identity;

    public class ApplicationUser : IdentityUser
    {
        public string AccountId { get; set; }

        public bool Disabled { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}