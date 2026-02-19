using Microsoft.EntityFrameworkCore;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{

	}

	public DbSet<ToDo> tbl_ToDo { get; set; }
	public DbSet<User> tbl_User { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ToDo>(opt =>
        {
            opt.Property(t => t.Title)
            .HasMaxLength(50)
            .IsRequired();

            opt.Property(t => t.Description)
            .HasMaxLength(200)
            .IsRequired();
        });

        modelBuilder.Entity<User>(u =>
        {
            u.Property(u => u.Name)
            .HasMaxLength(50)
            .IsRequired();

            u.Property(u => u.Email)
            .HasMaxLength(100)
            .IsRequired();

            u.HasIndex(u => u.Email)
            .IsUnique();

            u.Property(u => u.Password)
            .HasMaxLength(70)
            .IsRequired();

            u.Property(u => u.Role) 
            .HasMaxLength(20)
            .HasDefaultValue("User")
            .IsRequired();

            u.Property(u => u.RefreshToken)
            .HasMaxLength(200)
            .IsRequired(false);


            u.Property(u => u.RefreshTokenExpiresAt)
            .IsRequired(false);

            u.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        });
        

        base.OnModelCreating(modelBuilder);
    }
}
