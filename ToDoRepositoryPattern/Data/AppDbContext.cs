using Microsoft.EntityFrameworkCore;
using ToDoRepositoryPattern.Models;

namespace ToDoRepositoryPattern.Data;

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{

	}

	public DbSet<ToDo> tbl_ToDo { get; set; }

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

            //opt.Property(t => t.IsCompleted)
            //.HasDefaultValue(false);
        });

        base.OnModelCreating(modelBuilder);
    }
}
