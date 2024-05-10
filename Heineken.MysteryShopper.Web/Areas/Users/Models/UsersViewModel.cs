namespace SurveyUs.Web.Areas.Users.Models
{
    public class UsersViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
    }

    public class BartenderViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Ic { get; set; }
        public string Gender { get; set; }
        public string Telephone { get; set; }
        public string Outlet { get; set; }
        public string OutletAddress { get; set; }
        public string OutletLocation { get; set; }
        public string JoiningAs { get; set; }
        public string UniformSize { get; set; }
        public string Designation { get; set; }
        public bool IsVerified { get; set; }
        public bool IsDeleted { get; set; }
        public int Store { get; set; }
        public bool HeinekenTestStatus { get; set; }
        public bool GuinnessTestStatus { get; set; }
        public bool TheoryTestStatus { get; set; }
        public int TestId { get; set; }
    }
}