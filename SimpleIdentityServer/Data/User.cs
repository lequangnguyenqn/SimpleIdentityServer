using Microsoft.AspNetCore.Identity;

namespace SimpleIdentityServer.Data
{
    public class User : IdentityUser
    {
        public User()
        {
            Status = UserStatus.Inactive;
        }
        public UserStatus Status { get; set; }
    }
}
