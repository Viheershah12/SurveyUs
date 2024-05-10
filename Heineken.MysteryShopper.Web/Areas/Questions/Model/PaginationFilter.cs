namespace SurveyUs.Web.Areas.Questions.Model
{
    public class QuestionPaginationFilter
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public QuestionPaginationFilter()
        {
            PageNumber = 1;
            PageSize = 5;
        }
        public QuestionPaginationFilter(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber < 1 ? 1 : pageNumber;
            PageSize = pageSize > 5 ? 5 : pageSize;
        }
    }
}
