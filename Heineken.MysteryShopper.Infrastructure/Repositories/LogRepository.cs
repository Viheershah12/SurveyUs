using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.EntityFrameworkCore.AuditTrail.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SurveyUs.Application.DTOs.Logs;
using SurveyUs.Application.Interfaces.Repositories;
using SurveyUs.Application.Interfaces.Shared;

namespace SurveyUs.Infrastructure.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly IDateTimeService _dateTimeService;
        private readonly IMapper _mapper;
        private readonly IRepositoryAsync<Audit> _repository;

        public LogRepository(IRepositoryAsync<Audit> repository, IMapper mapper, IDateTimeService dateTimeService)
        {
            _repository = repository;
            _mapper = mapper;
            _dateTimeService = dateTimeService;
        }

        public async Task AddLogAsync(string action, string userId)
        {
            var audit = new Audit
            {
                Type = action,
                UserId = userId,
                DateTime = _dateTimeService.NowUtc
            };
            audit = await _repository.AddAsync(audit);
        }

        public async Task<List<AuditLogResponse>> GetAuditLogsAsync(string userId)
        {
            var logs = await _repository.Entities.Where(a => a.UserId == userId).OrderByDescending(a => a.Id).Take(250)
                .ToListAsync();
            var mappedLogs = _mapper.Map<List<AuditLogResponse>>(logs);
            return mappedLogs;
        }
    }

    public class LogProfile : Profile
    {
        public LogProfile()
        {
            CreateMap<AuditLogResponse, Audit>().ReverseMap();
        }
    }
}