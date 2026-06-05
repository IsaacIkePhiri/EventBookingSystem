using System;
using System.ComponentModel.DataAnnotations;

namespace EventEaseBookingSystem.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int VenueId { get; set; }

        public Venue? Venue { get; set; }

        public int EventTypeId { get; set; }

        public EventType? EventType { get; set; }
    }
}
