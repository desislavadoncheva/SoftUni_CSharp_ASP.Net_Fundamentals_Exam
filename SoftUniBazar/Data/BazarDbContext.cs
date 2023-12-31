﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data.Model;

namespace SoftUniBazar.Data
{
    public class BazarDbContext : IdentityDbContext
    {
        public BazarDbContext(DbContextOptions<BazarDbContext> options)
            : base(options)
        {
        }

        public DbSet<Ad> Ads { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AdBuyer> AdBuyers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AdBuyer>()
               .HasKey(b => new { b.BuyerId, b.AdId });

            modelBuilder.Entity<AdBuyer>()
                .HasOne(b => b.Buyer)
                .WithMany()
                .HasForeignKey(b => b.BuyerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AdBuyer>()
                .HasOne(a => a.Ad)
                .WithMany()
                .HasForeignKey(a => a.AdId)
                .OnDelete(DeleteBehavior.NoAction);


            modelBuilder
                .Entity<Category>()
                .HasData(new Category()
                {
                    Id = 1,
                    Name = "Books"
                },
                new Category()
                {
                    Id = 2,
                    Name = "Cars"
                },
                new Category()
                {
                    Id = 3,
                    Name = "Clothes"
                },
                new Category()
                {
                    Id = 4,
                    Name = "Home"
                },
                new Category()
                {
                    Id = 5,
                    Name = "Technology"
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}