// Source: Saini (2024), ASP.NET Core MVC Crash Course  
// Adapted for event management system  
// Original concept: Generic entity model with EF Core  


using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ST10435077___CLDV6211_POE.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }

        [Required]
        [StringLength(255)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.DateTime)] // Hint for UI frameworks
        public DateTime EventDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Event Type")]
        public string? EventType { get; set; }

        public string? Description { get; set; } // Nullable string

        // Foreign Key Property
        [Required]
        public int VenueId { get; set; }

        // Navigation property: Reference to the Venue where the event takes place
        [ForeignKey("VenueId")] // Links this navigation property to the VenueId foreign key
        public virtual Venue? Venue { get; set; } // Reference to the related Venue

        // Navigation property: An Event can have multiple Bookings
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
