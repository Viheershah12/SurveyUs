using System;
using SurveyUs.Application.Interfaces.Shared;

namespace SurveyUs.Infrastructure.Shared.Services
{
    public class SystemDateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}