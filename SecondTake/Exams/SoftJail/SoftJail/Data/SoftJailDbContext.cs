namespace SoftJail.Data
{
	using Microsoft.EntityFrameworkCore;
    using SoftJail.Data.Models;

    public class SoftJailDbContext : DbContext
	{

		public DbSet<Cell> Cells { get; set; }
		public DbSet<Department> Departments { get; set; }
		public DbSet<Mail> Mails { get; set; }
		public DbSet<Prisoner> Prisoners { get; set; }
		public DbSet<OfficerPrisoner> OfficersPrisoners { get; set; }
		public DbSet<Officer> Officers { get; set; }

		public SoftJailDbContext()
		{
		}

		public SoftJailDbContext(DbContextOptions options)
			: base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder
					.UseSqlServer(Configuration.ConnectionString);
			}
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<OfficerPrisoner>()
				.HasKey(x => new { x.PrisonerId, x.OfficerId });

			builder.Entity<Prisoner>()
				.HasMany(x => x.PrisonerOfficers)
				.WithOne(x => x.Prisoner)
				.OnDelete(DeleteBehavior.Restrict);

			builder.Entity<Officer>()
				.HasMany(x => x.OfficerPrisoners)
				.WithOne(x => x.Officer)
				.OnDelete(DeleteBehavior.Restrict);
		}
	}
}