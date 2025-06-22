﻿using System;
using System.Collections.Generic;

namespace ST10435077___CLDV6211_POE;

public partial class Venue
{
    public int VenueId { get; set; }

    public string VenueName { get; set; } = null!;

    public string? Location { get; set; }

    public int? Capacity { get; set; }

    public string? ImageUrl { get; set; } 

    public bool VenueAvailability { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
