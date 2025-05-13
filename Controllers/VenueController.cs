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
using ST10435077___CLDV6211_POE.Services;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace ST10435077___CLDV6211_POE.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseContext dbContext;
        private readonly BlobService _blobService;

        public VenueController(EventEaseContext dbContext, BlobService blobService)
        {
            this.dbContext = dbContext;
            _blobService = blobService;
        }

        [HttpGet]
        public IActionResult VenueAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VenueAdd(Venue viewModel, IFormFile imageFile)
        {
            const long maxFileSize = 2 * 1024 * 1024; // 2MB
            if (imageFile != null)
            {
                if (imageFile.Length > maxFileSize)
                {
                    ModelState.AddModelError("ImageUrl", "Image file size must be less than 2MB.");
                    return View(viewModel);
                }
                await _blobService.UploadAsync(imageFile);
                viewModel.ImageUrl = imageFile.FileName;
            }
            await dbContext.Venue.AddAsync(viewModel);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("VenueList");
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
        public async Task<IActionResult> VenueEdit(Venue viewModel, IFormFile imageFile)
        {
            const long maxFileSize = 2 * 1024 * 1024; // 2MB
            var venue = await dbContext.Venue.FindAsync(viewModel.VenueId);

            if (venue is not null)
            {
                venue.VenueName = viewModel.VenueName;
                venue.Location = viewModel.Location;
                venue.Capacity = viewModel.Capacity;
                if (imageFile != null)
                {
                    if (imageFile.Length > maxFileSize)
                    {
                        ModelState.AddModelError("ImageUrl", "Image file size must be less than 2MB.");
                        return View(viewModel);
                    }
                    await _blobService.UploadAsync(imageFile);
                    venue.ImageUrl = imageFile.FileName;
                }
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

        [HttpGet]
        public IActionResult BlobList()
        {
            var blobs = _blobService.ListBlobsAsync().Result;
            return View(blobs);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            var (stream, contentType) = await _blobService.DownloadAsync(fileName);
            return File(stream, contentType, fileName);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteImage(string fileName, int venueId)
        {
            await _blobService.DeleteAsync(fileName);
            var venue = await dbContext.Venue.FindAsync(venueId);
            if (venue != null && venue.ImageUrl == fileName)
            {
                venue.ImageUrl = null;
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("VenueEdit", new { VenueID = venueId });
        }
    }
}
