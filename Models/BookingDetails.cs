using System;

namespace EventEaseBookingSystem.Models
{
    public class BookingDetails
    {
        public int BookingId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public DateTime BookingDate { get; set; }

        public string VenueName { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public string EventTypeName { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
