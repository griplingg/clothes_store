using Microsoft.AspNetCore.Identity;

namespace ClothesWeb.Models
{
    public class UserRolesView
    {
        public IdentityUser User { get; set; }
        public IList<string> RoleNames { get; set; }
    }
}
