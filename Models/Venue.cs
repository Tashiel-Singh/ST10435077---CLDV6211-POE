// Source: Saini (2024), ASP.NET Core MVC Crash Course  
// Adapted for event management system  
// Original concept: Generic entity model with EF Core  


using System.ComponentModel.DataAnnotations;

namespace ST10435077___CLDV6211_POE.Models
{
    public class Venue
    {
        [Key] // Marks VenueId as the primary key
        public int VenueId { get; set; }

        [Required] // Makes VenueName mandatory
        [StringLength(255)] // Sets max length, corresponds to NVARCHAR(255)
        public string VenueName { get; set; } = string.Empty; // Initialize to avoid null warnings

        [StringLength(500)] // Sets max length
        public string? Location { get; set; } // Nullable string

        public int? Capacity { get; set; } // Nullable integer

        [DataType(DataType.ImageUrl)] // Hint for UI frameworks
        public string? ImageUrl { get; set; } // Nullable string

        // Navigation property: A Venue can host multiple Events
        public virtual ICollection<Event> Event { get; set; } = new List<Event>();

        // Navigation property: A Venue can be associated with multiple Bookings
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedDate { get; set; }

        public string? LastModifiedBy { get; set; }
    }
}
