

using Microsoft.AspNetCore.Identity;

namespace test_mvc_webapp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName {get; set;}
        public string LastName {get; set;}
    }
}