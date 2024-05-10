using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCoreHero.Abstractions.Domain;
using AspNetCoreHero.EntityFrameworkCore.AuditTrail;
using Microsoft.EntityFrameworkCore;
using SurveyUs.Application.Interfaces.Contexts;
using SurveyUs.Application.Interfaces.Shared;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Infrastructure.DbContexts
{
    public class ApplicationDbContext : AuditableContext, IApplicationDbContext
    {
        private readonly IAuthenticatedUserService _authenticatedUser;
        private readonly IDateTimeService _dateTime;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime,
            IAuthenticatedUserService authenticatedUser) : base(options)
        {
            _dateTime = dateTime;
            _authenticatedUser = authenticatedUser;
        }

        public DbSet<UserExtension> UserExtension { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreMappings> StoreMappings { get; set; }
        public DbSet<Campaign> Campaign { get; set; }
        public DbSet<CampaignMappings> CampaignMappings { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionMappings> QuestionMappings { get; set; }
        public DbSet<QuestionChoices> QuestionChoices { get; set; }
        public DbSet<UserAnswers> UserAnswers { get; set; }
        public DbSet<QuestionCategory> QuestionCategory { get; set; }
        public DbSet<QuestionCategoryMapping> QuestionCategoryMapping { get; set; }
        public DbSet<QuestionMedia> QuestionMedia { get; set; }
        public DbSet<QuestionAnswers> QuestionAnswers { get; set; }
        public IDbConnection Connection => Database.GetDbConnection();

        public bool HasChanges => ChangeTracker.HasChanges();

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>().ToList())
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = _authenticatedUser.UserId;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = _authenticatedUser.UserId;
                        break;
                }

            if (_authenticatedUser.UserId == null)
                return await base.SaveChangesAsync(cancellationToken);
            return await base.SaveChangesAsync(_authenticatedUser.UserId);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            foreach (var property in builder.Model.GetEntityTypes()
                         .SelectMany(t => t.GetProperties())
                         .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                property.SetColumnType("decimal(18,2)");
            base.OnModelCreating(builder);
        }
    }
}