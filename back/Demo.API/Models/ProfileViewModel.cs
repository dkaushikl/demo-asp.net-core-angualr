namespace Demo.API.Models
{
    public class ProfileViewModel
    {
        public ProfileViewModel()
        {
        }

        public ProfileViewModel(ApplicationUser user)
        {
            this.Id = user.Id;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
        }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }
    }
}