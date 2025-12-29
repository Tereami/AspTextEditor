using Microsoft.AspNetCore.Identity;
using System;

namespace AspTextEditor.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;


    }
}
