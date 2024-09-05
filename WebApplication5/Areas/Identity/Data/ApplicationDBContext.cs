using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication5.Models.Entities;

namespace WebApplication5.Data;

public class ApplicationDBContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
        : base(options)
    {
    }
    public DbSet<Product> Product { get; set; }

    public DbSet<Catagory> Catagory { get; set; }

    public DbSet<ShoppingCart> ShoppingCart { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<Product>().ToTable("Products");
        builder.Entity<Catagory>().ToTable("Catagories");
        builder.Entity<ShoppingCart>().ToTable("ShoppingCarts");

        builder.Entity<IdentityRole>().HasData(
            new IdentityRole { Id = "e46b105e-984b-47f3-836e-9139b643eb5f", Name = "ADMIN", NormalizedName = "ADMIN",ConcurrencyStamp= "9A591482-741A-45DA-8461-020F0EA00E89" }
        );
        builder.Entity<IdentityUser>().HasData(
            new IdentityUser { Id = "034a4d62-481c-42c5-8e06-24c20e24fff6",UserName = "ektabothra07@gmail.com", Email = "ektabothra07@gmail.com", EmailConfirmed = true, NormalizedEmail = "EKTABOTHRA07@GMAIL.COM", NormalizedUserName = "EKTABOTHRA07@GMAIL.COM", PasswordHash = "AQAAAAIAAYagAAAAEBzR8pDvdH27zvml1Glvljsu+zjR+QPGTeAp1HF1bmeP13D4KAwZVv9z9kBNfEGxQA==", SecurityStamp = "RCLJPHIHNU6LQWQ7CQBWRT3TS6IZ5T7J", ConcurrencyStamp = "444f9e03-9f54-4dd7-9473-7530573e2ab6" }

        );
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string> { UserId = "034a4d62-481c-42c5-8e06-24c20e24fff6", RoleId = "e46b105e-984b-47f3-836e-9139b643eb5f" }
        );


    }
}
