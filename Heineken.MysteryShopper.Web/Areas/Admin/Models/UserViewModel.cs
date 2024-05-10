using System;
using System.Collections.Generic;

namespace SurveyUs.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsMysteryShopper { get; set; } = false;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        public string Role { get; set; }
        public int StoreId { get; set; }
        public bool IsAssigned { get; set; }
        public string DataTableId { get; set; }
        public DateTime CreatedOn {  get; set; }
    }

    public class ParticipantViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
        public string Ic { get; set; }
        public string Gender { get; set; }
        public string Telephone { get; set; }
        public string OutletName { get; set; }
        public string OutletAddress { get; set; }
        public string OutletLocation { get; set; }
        public string JoiningAs { get; set; }
        public string UniformSize { get; set; }
        public string Designation { get; set; }
        public int Store { get; set; }
        public List<StoreOption> Stores { get; set; }
        public List<string> ExclusionList { get; set; }
    }

    public class StoreOption
    {
        public int StoreId { get; set; }
        public string StoreName { get; set; }
    }
}