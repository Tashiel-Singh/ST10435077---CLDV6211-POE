// Author: Microsoft Corporation  
// Year: 2023  
// Title: ASP.NET Core MVC Framework  
// Source: ASP.NET Core Documentation  
// URL: https://docs.microsoft.com/en-us/aspnet/core  
// [Accessed: 2025]  
using Microsoft.AspNetCore.Mvc;

// Author: PostgreSQL Global Development Group  
// Year: 2023  
// Title: Entity Framework Core  
// Source: Microsoft Entity Framework Core Documentation  
// URL: https://docs.microsoft.com/en-us/ef/core/  
// [Accessed: 2025]  
using Microsoft.EntityFrameworkCore;
using ST10435077___CLDV6211_POE.Models;
using System.Linq;

namespace ST10435077___CLDV6211_POE.Controllers
{
    [Route("[controller]")]
    public class BookingController : Controller
    {
        private readonly EventEaseContext dbContext;

        public BookingController(EventEaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        [Route("BookingAdd")]
        public IActionResult BookingAdd()
        {
            return View();
        }

        [HttpPost]
        [Route("BookingAdd")]
        public async Task<IActionResult> BookingAdd(Booking viewModel)
        {
            try
            {
                // Check for double booking
                var isDoubleBooked = dbContext.Booking.Any(b => b.VenueId == viewModel.VenueId &&
                                                                b.BookingDate.Date == viewModel.BookingDate.Date &&
                                                                b.BookingDate.TimeOfDay == viewModel.BookingDate.TimeOfDay);

                if (isDoubleBooked)
                {
                    ModelState.AddModelError("", "This venue is already booked for the selected date and time.");
                    return View(viewModel);
                }

                var booking = new Booking
                {
                    EventId = viewModel.EventId,
                    VenueId = viewModel.VenueId,
                    BookingDate = DateTime.UtcNow, // Auto-set creation time
                    Status = viewModel.Status
                };

                if (ModelState.IsValid)
                {
                    await dbContext.Booking.AddAsync(booking);
                    await dbContext.SaveChangesAsync();
                    return RedirectToAction("BookingList");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the booking: " + ex.Message);
            }

            return View(viewModel);
        }

        [HttpGet]
        [Route("")]
        [Route("BookingList")]
        public async Task<IActionResult> BookingList()
        {
            try 
            {
                var bookings = await dbContext.Booking
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .ToListAsync();
                return View(bookings);
            }
            catch 
            {
                return View("Error");
            }
        }

        [HttpGet]
        [Route("BookingEdit/{BookingID}")]
        public async Task<IActionResult> BookingEdit(int BookingID)
        {
            var booking = await dbContext.Booking
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(b => b.BookingId == BookingID);

            return View(booking);
        }

        [HttpPost]
        [Route("BookingEdit")]
        public async Task<IActionResult> BookingEdit(Booking viewModel)
        {
            var booking = await dbContext.Booking.FindAsync(viewModel.BookingId);

            if (booking is not null)
            {
                booking.EventId = viewModel.EventId;
                booking.VenueId = viewModel.VenueId;
                booking.Status = viewModel.Status;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("BookingList");
        }

        [HttpPost]
        [Route("DeleteBooking/{BookingId}")]
        public async Task<IActionResult> DeleteBooking(int BookingId)
        {
            var booking = await dbContext.Booking.FindAsync(BookingId);

            if (booking is not null)
            {
                dbContext.Booking.Remove(booking);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("BookingList");
        }
    }
}