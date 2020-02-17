using BandAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BandAPI.DbContexts
{
    public class BandAlbumContext : DbContext
    {
        public BandAlbumContext(DbContextOptions<BandAlbumContext> options) :
            base(options)
        {

        }

        public DbSet<Band> Bands { get; set; } // Bands Dataset
        public DbSet<Album> Albums { get; set; } // Albums Dataset

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Seeding the data
            modelBuilder.Entity<Band>().HasData(new Band()
            {
                Id = Guid.Parse("12a1c828-aaa3-4940-98ab-3625347fac3a"),
                Name = "Band 2",
                Founded = new DateTime(1980, 1, 1),
                MainGenre = "Heavy Metal"

            }, new Band()
            {
                Id = Guid.Parse("ed69f31c-7475-45c0-a062-55f7c7245af5"),
                Name = "Band 3",
                Founded = new DateTime(1980, 1, 2),
                MainGenre = "Pop"

            }, new Band()
            {
                Id = Guid.Parse("50b56f93-20b9-4b49-96ce-5556d2ccee6d"),
                Name = "Band 4",
                Founded = new DateTime(1980, 1, 3),
                MainGenre = "Style 3"

            }, new Band()
            {
                Id = Guid.Parse("6ff5c7b0-5465-4e21-a346-4564b34dda62"),
                Name = "Band 5",
                Founded = new DateTime(1980, 1, 4),
                MainGenre = "Style 4"

            }, new Band()
            {
                Id = Guid.Parse("cf7e2071-cadc-4cfa-a197-d37a708be991"),
                Name = "Band 6",
                Founded = new DateTime(1980, 1, 5),
                MainGenre = "Style 5"

            });

            modelBuilder.Entity<Album>().HasData(new Album()
            {
                Id = Guid.Parse("fad373fc-f59e-4666-a6d0-ce84abad4ce2"),
                Title = "Master of Puppets",
                Description = "One of the best heavy metal albums ever",
                BandId = Guid.Parse("cf7e2071-cadc-4cfa-a197-d37a708be991"),
            }, new Album()
            {
                Id = Guid.Parse("3e6e88e7-18f2-4532-8320-b81a09aee05f"),
                Title = "King in the Heaven",
                Description = "Legen Soul Music",
                BandId = Guid.Parse("cf7e2071-cadc-4cfa-a197-d37a708be991"),
            }, new Album()
            {
                Id = Guid.Parse("008f39cc-ae29-44f2-8929-9b13fda96079"),
                Title = "Green Piano",
                Description = "Classic Piano Performance",
                BandId = Guid.Parse("12a1c828-aaa3-4940-98ab-3625347fac3a"),
            }, new Album()
            {
                Id = Guid.Parse("bfcb9600-907f-4606-ae2e-ed11db784eb9"),
                Title = "Lion King",
                Description = "Ophera Music",
                BandId = Guid.Parse("6ff5c7b0-5465-4e21-a346-4564b34dda62"),
            }, new Album()
            {
                Id = Guid.Parse("9b4d14b1-40d4-4886-8b6b-58d8f3cf173f"),
                Title = "Albumn Name",
                Description = "Testing Description",
                BandId = Guid.Parse("ed69f31c-7475-45c0-a062-55f7c7245af5"),
            });

            base.OnModelCreating(modelBuilder);

        }
    }
}
