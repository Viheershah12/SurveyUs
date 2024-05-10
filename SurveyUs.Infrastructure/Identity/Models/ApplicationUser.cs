using System;
using Microsoft.AspNetCore.Identity;

namespace SurveyUs.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}