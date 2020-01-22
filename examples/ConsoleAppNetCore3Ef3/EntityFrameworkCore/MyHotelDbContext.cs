using System;
using ConsoleAppNetCore3Ef3.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleAppNetCore3Ef3.EntityFrameworkCore
{
    public class MyHotelDbContext : DbContext
    {
        public MyHotelDbContext(DbContextOptions<MyHotelDbContext> options)
            : base(options)
        {
            // this.ChangeTracker.LazyLoadingEnabled = false;
        }

        public DbSet<Reservation> Reservations { get; set; }

        public DbSet<Guest> Guests { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomDetail> RoomDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //GUESTS
            modelBuilder.Entity<Guest>().HasData(new Guest("Alper Ebicoglu", DateTime.Now.AddDays(-10)) { Id = 1 });
            modelBuilder.Entity<Guest>().HasData(new Guest("George Michael", DateTime.Now.AddDays(-5)) { Id = 2 });
            modelBuilder.Entity<Guest>().HasData(new Guest("Daft Punk", DateTime.Now.AddDays(-1)) { Id = 3 });

            //ROOMDETAILS
            modelBuilder.Entity<RoomDetail>().HasData(new RoomDetail(2, 1) { Id = 100 });
            modelBuilder.Entity<RoomDetail>().HasData(new RoomDetail(4, 1) { Id = 101 });
            modelBuilder.Entity<RoomDetail>().HasData(new RoomDetail(3, 2) { Id = 102 });
            modelBuilder.Entity<RoomDetail>().HasData(new RoomDetail(0, 2) { Id = 103 });

            //ROOMS
            modelBuilder.Entity<Room>().HasData(new Room(101, "yellow-room", RoomStatus.Available, false, 100) { Id = 1 });
            modelBuilder.Entity<Room>().HasData(new Room(102, "blue-room", RoomStatus.Available, false, 101) { Id = 2 });
            modelBuilder.Entity<Room>().HasData(new Room(103, "white-room", RoomStatus.Unavailable, false, 102) { Id = 3 });
            modelBuilder.Entity<Room>().HasData(new Room(104, "black-room", RoomStatus.Unavailable, false, 103) { Id = 4 });

            //RESERVATIONS
            modelBuilder.Entity<Reservation>().HasData(new Reservation(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(3), 3, 1) { Id = 1 });
            modelBuilder.Entity<Reservation>().HasData(new Reservation(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(4), 4, 2) { Id = 2 });

            base.OnModelCreating(modelBuilder);
        }
    }
}
