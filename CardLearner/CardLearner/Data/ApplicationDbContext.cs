using CardLearner.Models;
using Microsoft.EntityFrameworkCore;

namespace CardLearner.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserProgress> UserProgresses => Set<UserProgress>();
}

