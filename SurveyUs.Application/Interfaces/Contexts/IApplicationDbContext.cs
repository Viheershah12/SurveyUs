using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SurveyUs.Domain.Entities;

namespace SurveyUs.Application.Interfaces.Contexts
{
    public interface IApplicationDbContext
    {
        IDbConnection Connection { get; }
        bool HasChanges { get; }

        DbSet<UserExtension> UserExtension { get; set; }
        DbSet<Store> Store { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<QuestionMappings> QuestionMappings { get; set; }
        public DbSet<QuestionChoices> QuestionChoices { get; set; }
        public DbSet<UserAnswers> UserAnswers { get; set; }
        public DbSet<QuestionCategory> QuestionCategory { get; set; }
        public DbSet<QuestionCategoryMapping> QuestionCategoryMapping { get; set; }

        EntityEntry Entry(object entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}