using Checklist_API.Features.Checklists.Entity;
using Checklist_API.Features.JWT.Entity;
using Checklist_API.Features.Users.Entity;
using Microsoft.EntityFrameworkCore;

namespace Check_List_API.Data;

public class CheckListDbContext : DbContext
{
    public CheckListDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<CheckList> CheckList { get; set; }
    public DbSet<User> User { get; set; }
    public DbSet<Role> Role { get; set; }
    public DbSet<UserRole> UserRole { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Mulig fix for capital letter issue tables when using docker:
        //modelBuilder.Entity<CheckList>().ToTable("Checklist");
        //modelBuilder.Entity<User>().ToTable("User");
        //modelBuilder.Entity<JWTRole>().ToTable("Jwtrole");
        //modelBuilder.Entity<JWTUserRole>().ToTable("Jwtuserrole");

        base.OnModelCreating(modelBuilder);

        // seed roles into JWTRole table
        modelBuilder.Entity<Role>().HasData(
        new Role { RoleName = "Admin" },
        new Role { RoleName = "User" });

        #region CheckList

      modelBuilder.Entity<CheckList>()  // Strongly typed id deklarert
            .Property(x => x.Id)
            .HasConversion(
                id => id.checklistId,
                value => new ChecklistId(value)
         );

        modelBuilder.Entity<CheckList>()  // Strongly typed id deklarert
            .Property(x => x.UserId)
            .HasConversion(
               id => id.userId,
               value => new UserId(value)
         );

        modelBuilder.Entity<CheckList>() 
            .HasKey(x => x.Id);

        modelBuilder.Entity<CheckList>() 
            .HasOne(c => c.User)
            .WithMany(u => u.Checklists)
            .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<CheckList>()
           .Property(x => x.Id)
           .IsRequired();

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.Description)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.Status)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.Priority)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.AssignedTo)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.Comments)
            .IsRequired()
            .HasMaxLength(100);

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.DueDate)
            .IsRequired();


        modelBuilder.Entity<CheckList>() 
            .Property(x => x.DateCreated)
            .IsRequired();

        modelBuilder.Entity<CheckList>() 
            .Property(x => x.DateUpdated)
            .IsRequired(); 
        
        modelBuilder.Entity<CheckList>() 
            .Property(x => x.DateCompleted)
            .IsRequired();

        #endregion

        #region User

        modelBuilder.Entity<User>()
          .Property(x => x.Id)
          .HasConversion(
              id => id.userId,
              value => new UserId(value)
          );

        modelBuilder.Entity<User>()
          .HasKey(x => x.Id);

        modelBuilder.Entity<User>()
           .HasMany(u => u.Checklists)
           .WithOne(c => c.User)
           .HasForeignKey(c => c.UserId);

        modelBuilder.Entity<User>()
           .HasMany(u => u.UserRoles)
           .WithOne(ur => ur.User)
           .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<User>()
          .Property(x => x.Id)
          .IsRequired();

        modelBuilder.Entity<User>()
          .Property(x => x.FirstName)
          .HasMaxLength(100)
          .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.LastName)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.PhoneNumber)
            .HasMaxLength(8)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(x => x.Email)
            .HasMaxLength(30)
            .IsRequired();

        modelBuilder.Entity<User>()
           .Property(x => x.HashedPassword)
           .IsRequired();

        modelBuilder.Entity<User>()
           .Property(x => x.Salt)
           .IsRequired();

        modelBuilder.Entity<User>() 
            .Property(x => x.DateCreated)
            .IsRequired();

        modelBuilder.Entity<User>() 
            .Property(x => x.DateUpdated)
            .IsRequired();

        #endregion

        #region Role       

        modelBuilder.Entity<Role>()
            .HasKey(x => x.RoleName);      

        modelBuilder.Entity<Role>()
            .HasMany(x => x.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(j => j.RoleName);

        modelBuilder.Entity<Role>()
           .Property(x => x.RoleName)
           .IsRequired();

        modelBuilder.Entity<Role>()
            .Property(x => x.RoleName)
            .HasMaxLength(20)
            .IsRequired();

        #endregion

        #region UserRole       

        modelBuilder.Entity<UserRole>()
         .Property(x => x.UserId)
         .HasConversion(
               id => id.userId,
               value => new UserId(value)
         );

        modelBuilder.Entity<UserRole>()
         .HasKey(ur => new { ur.RoleName, ur.UserId }); // Composite key

        modelBuilder.Entity<UserRole>()
         .HasOne(j => j.Role)
         .WithMany(j => j.UserRoles)
         .HasForeignKey(j => j.RoleName);

        modelBuilder.Entity<UserRole>()
         .HasOne(u => u.User)
         .WithMany(j => j.UserRoles)
         .HasForeignKey(u => u.UserId);

        modelBuilder.Entity<UserRole>()
         .Property(x => x.DateCreated)
         .IsRequired();

        modelBuilder.Entity<UserRole>()
         .Property(x => x.DateUpdated)
         .IsRequired();
        #endregion
    }
}
