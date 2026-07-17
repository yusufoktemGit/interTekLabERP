using interTekLabERP.Entities;
using InterTekLabERP.Entities.Domain;
using Microsoft.EntityFrameworkCore;

namespace interTekLabERP.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<SampleRequest> SampleRequests => Set<SampleRequest>();

    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<SampleHistory> SampleHistories => Set<SampleHistory>();

    public DbSet<TestCard> TestCards => Set<TestCard>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SampleRequest>()
            .HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId);

        modelBuilder.Entity<SampleRequest>()
            .HasOne(x => x.CreatedByUser)
            .WithMany()
            .HasForeignKey(x => x.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SampleRequest>()
            .HasOne(x => x.UpdatedByUser)
            .WithMany()
            .HasForeignKey(x => x.UpdatedBy)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SampleHistory>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<SampleHistory>()
            .ToTable("SampleHistory");
    }
}