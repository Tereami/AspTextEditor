using AspTextEditor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace AspTextEditor.Data
{
    public class DB : IdentityDbContext<AppUser>
    {
        public DB(DbContextOptions options) : base(options) { }
    }
}
