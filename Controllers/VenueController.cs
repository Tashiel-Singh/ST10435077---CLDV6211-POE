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
    public class VenueController : Controller
    {
        private readonly EventEaseContext dbContext;

        public VenueController(EventEaseContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult VenueAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VenueAdd(Venue viewModel)
        {
            var venue = new Venue
            {
                VenueName = viewModel.VenueName,
                Location = viewModel.Location,
                Capacity = viewModel.Capacity,
                ImageUrl = viewModel.ImageUrl
            };

            await dbContext.Venue.AddAsync(venue);
            await dbContext.SaveChangesAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VenueList()
        {
            var venue = await dbContext.Venue.ToListAsync();
            return View(venue);  // Return the venue to the view
        }

        [HttpGet]
        public async Task<IActionResult> VenueEdit(int VenueID)
        {
            var venue = await dbContext.Venue.FindAsync(VenueID);

            return View(venue);
        }

        [HttpPost]
        public async Task<IActionResult> VenueEdit(Venue viewModel)
        {
            var venue = await dbContext.Venue.FindAsync(viewModel.VenueId);

            if (venue is not null)
            {
                venue.VenueName = viewModel.VenueName;
                venue.Location = viewModel.Location;
                venue.Capacity = viewModel.Capacity;
                venue.ImageUrl = viewModel.ImageUrl;

                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("VenueList");
        }

        [HttpGet]
        public async Task<IActionResult> VenueDelete(int venueId)
        {
            var venue = await dbContext.Venue.FindAsync(venueId);
            if (venue == null)
            {
                return NotFound();
            }
            return View(venue);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            try
            {
                var venue = dbContext.Venue.Include(v => v.Bookings).FirstOrDefault(v => v.VenueId == id);

                if (venue == null)
                {
                    return NotFound();
                }

                if (venue.Bookings.Any())
                {
                    ModelState.AddModelError("", "This venue cannot be deleted as it is associated with active bookings.");
                    return RedirectToAction("VenueList");
                }

                dbContext.Venue.Remove(venue);
                dbContext.SaveChanges();
                return RedirectToAction("VenueList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while deleting the venue: " + ex.Message);
                return RedirectToAction("VenueList");
            }
        }
    }
}
