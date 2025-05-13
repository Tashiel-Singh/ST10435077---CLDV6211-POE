using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ST10435077___CLDV6211_POE.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        // Foreign Key Property for Event
        [Required]
        public int EventId { get; set; }

        // Foreign Key Property for Venue
        [Required]
        public int VenueId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow; // Current time to enable the database to record the time a booking was made

        [Required]
        public string Status { get; set; } = string.Empty; // Initialize to avoid null warnings 

        // Navigation property: Reference to the Event being booked
        [ForeignKey("EventId")]
        public virtual Event? Event { get; set; } // Reference to the related Event

        // Navigation property: Reference to the Venue associated with the booking
        // (Even though Event links to Venue, having it here can simplify some queries)
        [ForeignKey("VenueId")]
        public virtual Venue? Venue { get; set; } // Reference to the related Venue
    }
}
