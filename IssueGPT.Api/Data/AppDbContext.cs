using Microsoft.EntityFrameworkCore;
using IssueGPT.Api.Models;

namespace IssueGPT.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Repository> Repositories { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<IssueComment> IssueComments { get; set; }
    public DbSet<Analysis> Analyses { get; set; }
    public DbSet<CopilotPrompt> CopilotPrompts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Repository -> Issue (1-to-many)
        modelBuilder.Entity<Issue>()
            .HasOne(i => i.Repository)
            .WithMany(r => r.Issues)
            .HasForeignKey(i => i.RepositoryId)
            .OnDelete(DeleteBehavior.Cascade);

        // Issue -> IssueComment (1-to-many)
        modelBuilder.Entity<IssueComment>()
            .HasOne(c => c.Issue)
            .WithMany(i => i.Comments)
            .HasForeignKey(c => c.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Issue -> Analysis (1-to-many)
        modelBuilder.Entity<Analysis>()
            .HasOne(a => a.Issue)
            .WithMany(i => i.Analyses)
            .HasForeignKey(a => a.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        // Analysis -> CopilotPrompt (1-to-many)
        modelBuilder.Entity<CopilotPrompt>()
            .HasOne(cp => cp.Analysis)
            .WithMany(a => a.CopilotPrompts)
            .HasForeignKey(cp => cp.AnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<Issue>()
            .HasIndex(i => new { i.RepositoryId, i.GitHubIssueNumber })
            .IsUnique();

        modelBuilder.Entity<Analysis>()
            .HasIndex(a => a.IssueId);

        modelBuilder.Entity<CopilotPrompt>()
            .HasIndex(cp => cp.AnalysisId);
    }
}
