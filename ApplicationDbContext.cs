using EventEaseBookingSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EventEaseBookingSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venues { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventType> EventTypes { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<BookingDetails> BookingDetails { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, Name = "Wedding" },
                new EventType { EventTypeId = 2, Name = "Conference" },
                new EventType { EventTypeId = 3, Name = "Birthday" },
                new EventType { EventTypeId = 4, Name = "Concert" },
                new EventType { EventTypeId = 5, Name = "Corporate" },
                new EventType { EventTypeId = 6, Name = "Workshop" }
            );

            modelBuilder.Entity<Event>()
                .HasOne(e => e.Venue)
                .WithMany()
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Event>()
                .HasOne(e => e.EventType)
                .WithMany(et => et.Events)
                .HasForeignKey(e => e.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Venue)
                .WithMany()
                .HasForeignKey(b => b.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Event)
                .WithMany()
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingDetails>()
                .HasNoKey()
                .ToView("BookingDetails");
        }
    }
}
