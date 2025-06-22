using System;
using System.Collections.Generic;

namespace ST10435077___CLDV6211_POE;

public partial class Event
{
    public int EventId { get; set; }

    public string EventName { get; set; } = null!;

    public string? EventType { get; set; }

    public DateTime EventDate { get; set; }

    public string? Description { get; set; }

    public int VenueId { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Venue Venue { get; set; } = null!;
}
