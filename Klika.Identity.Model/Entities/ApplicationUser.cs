using System;
using EntityFrameworkCore.Triggers;
using Microsoft.AspNetCore.Identity;

namespace Klika.Identity.Model.Entities
{
    public class ApplicationUser : IdentityUser, ITrackable
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Triggers are directly used here because ApplicationUser can't derive from Trackable, use Trackable for other entities
        public ApplicationUser()
        {
            Triggers<ApplicationUser>.Updating += entry => entry.Entity.UpdatedAt = DateTime.Now;
        }
    }
}
