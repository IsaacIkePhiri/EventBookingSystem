using System;
using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Booking
    {
        public int BookingId { get; set; }

        public int VenueId { get; set; }

        public int EventId { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public Venue? Venue { get; set; }

        public Event? Event { get; set; }
    }
}