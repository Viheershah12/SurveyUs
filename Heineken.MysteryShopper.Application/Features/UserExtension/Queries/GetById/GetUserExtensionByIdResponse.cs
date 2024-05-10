namespace SurveyUs.Application.Features.UserExtension.Queries.GetById
{
    public class GetUserExtensionByIdResponse
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
    }
}