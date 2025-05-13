using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ST10435077___CLDV6211_POE;

public partial class EventEaseContext : DbContext
{
    public EventEaseContext()
    {
    }

    public EventEaseContext(DbContextOptions<EventEaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Booking { get; set; }

    public virtual DbSet<Event> Event { get; set; }

    public virtual DbSet<Venue> Venue { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=tcp:eventease-st10435077.database.windows.net,1433;Initial Catalog=EventEase;Persist Security Info=False;User ID=EventEase1;Password=Cyrus123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {


        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Booking__73951AEDECB9C6CD");

            entity.ToTable("Booking");

            entity.Property(e => e.BookingDate).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.EventId)
                .HasConstraintName("FK_Booking_Event");

            entity.HasOne(d => d.Venue).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Booking_Venue");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C8104A316522");

            entity.ToTable("Event");

            entity.Property(e => e.EventName).HasMaxLength(255);

            entity.HasOne(d => d.Venue).WithMany(p => p.Events)
                .HasForeignKey(d => d.VenueId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Event_Venue");
        });

        modelBuilder.Entity<Venue>(entity =>
        {
            entity.HasKey(e => e.VenueId).HasName("PK__Venue__3C57E5F2C0E7ADCF");

            entity.ToTable("Venue");

            entity.Property(e => e.Location).HasMaxLength(500);
            entity.Property(e => e.VenueName).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
