﻿namespace SurveyUs.Application.Features.QuestionMappings.Queries.GetAllCached
{
    public class GetAllQuestionMappingsCachedResponse
    {
        public int Id { get; set; }

        public int CampaignId { get; set; }

        public int QuestionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
