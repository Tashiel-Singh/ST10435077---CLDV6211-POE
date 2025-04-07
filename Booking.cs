using System;
using System.Collections.Generic;

namespace ST10435077___CLDV6211_POE;

public partial class Booking
{
    public int BookingId { get; set; }

    public int EventId { get; set; }

    public int VenueId { get; set; }

    public DateTime BookingDate { get; set; }

    public string Status { get; set; } = null!;
    public virtual Event Event { get; set; } = null!;

    public virtual Venue Venue { get; set; } = null!;
    
}
