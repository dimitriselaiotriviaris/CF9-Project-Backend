using Microsoft.EntityFrameworkCore;
using CF9Project.Models;

namespace CF9Project.Data;

public class CF9ProjectContext : DbContext
{

    public CF9ProjectContext(DbContextOptions<CF9ProjectContext> options)
        : base(options)
    {
    }

    public DbSet<Capability> Capabilities { get; set; }

    public DbSet<Game> Games { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Gamer> Gamers { get; set; }

    public DbSet<Company> Companies { get; set; }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Capability>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            //entity.HasIndex(e => e.Name, "IX_Capabilities_Name");
            entity.HasIndex(e => e.Name, "UQ_Capabilities_Name").IsUnique();
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Price).HasDefaultValue(0);

            entity.HasOne(d => d.Company)
                .WithMany(p => p.Games)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Games_CompanyId");

            // Τα πεδία του Composite Key ονομάζονται by default StudentsId και CoursesId
            entity.HasMany(d => d.Gamers).WithMany(p => p.Games)
                    .UsingEntity("GamersGames");

            // Αν θέλουμε explicit ονομασίες πεδίων:
            //entity.HasMany(d => d.Students).WithMany(p => p.Courses)
            //    .UsingEntity("StudentsCourses",
            //        l => l.HasOne(typeof(Student)).WithMany().HasForeignKey("StudentId"),
            //        r => r.HasOne(typeof(Course)).WithMany().HasForeignKey("CourseId"));

            entity.HasIndex(e => e.Description, "IX_Games_Description");
            entity.HasIndex(e => e.CompanyId, "IX_Games_CompanyId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasMany(d => d.Capabilities).WithMany(p => p.Roles)  // (RolesId, CapabilitiesId)
                .UsingEntity("RolesCapabilities", j =>
                {
                    j.HasIndex("CapabilitiesId")
                    .HasDatabaseName("IX_RolesCapabilities_CapabilityId");
                });
            //entity.HasIndex(e => e.Name, "IX_Roles_Name");
            entity.HasIndex(e => e.Name, "UQ_Roles_Name").IsUnique();
        });

        modelBuilder.Entity<Gamer>(entity =>
        { 
            entity.HasOne(d => d.User).WithOne(p => p.Gamer)
                .HasForeignKey<Gamer>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Gamers_UserId");

            entity.HasIndex(e => e.UserId, "IX_Gamers_UserId").IsUnique();
        });

        modelBuilder.Entity<Company>(entity =>
        { 

            entity.HasOne(d => d.User).WithOne(p => p.Company) 
                .HasForeignKey<Company>(d => d.UserId)
                //  Ισχύει ούτως ή αλλιώς από το EF. Αλλά δεν πειράζει να είναι explicit για readability.
                .OnDelete(DeleteBehavior.Cascade)   
                .HasConstraintName("FK_Companies_UserId");

            entity.HasIndex(e => e.UserId, "IX_Companies_UserId").IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(60);
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Users_RoleId");

            entity.HasIndex(e => e.Email, "IX_Users_Email").IsUnique();
            entity.HasIndex(e => e.RoleId, "IX_Users_RoleId");
            entity.HasIndex(e => e.Username, "IX_Users_Username").IsUnique();
        });
    }
}
