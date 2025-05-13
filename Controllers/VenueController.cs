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
using System.Diagnostics;

namespace ST10435077___CLDV6211_POE.Controllers
{
    [Route("[controller]")]
    public class VenueController : Controller
    {
        private readonly EventEaseContext dbContext;
        private readonly BlobService _blobService;
        private readonly ILogger<VenueController> _logger;

        public VenueController(EventEaseContext dbContext, BlobService blobService, ILogger<VenueController> logger)
        {
            this.dbContext = dbContext;
            _blobService = blobService;
            _logger = logger;
        }

        [HttpGet]
        [Route("")]
        [Route("VenueList")]
        public async Task<IActionResult> VenueList()
        {
            try 
            {
                var venues = await dbContext.Venue.ToListAsync();
                return View(venues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving venue list");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the venue list. Please try again later." });
            }
        }

        [HttpGet]
        [Route("VenueAdd")]
        public IActionResult VenueAdd()
        {
            return View();
        }

        [HttpPost]
        [Route("VenueAdd")]
        public async Task<IActionResult> VenueAdd(Venue viewModel, IFormFile imageFile)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding venue");
                ModelState.AddModelError("", "An error occurred while saving the venue. Please try again.");
                return View(viewModel);
            }
        }

        [HttpGet]
        [Route("VenueEdit/{VenueID}")]
        public async Task<IActionResult> VenueEdit(int VenueID)
        {
            try
            {
                var venue = await dbContext.Venue.FindAsync(VenueID);
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving venue for editing");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the venue. Please try again later." });
            }
        }

        [HttpPost]
        [Route("VenueEdit")]
        public async Task<IActionResult> VenueEdit(Venue viewModel, IFormFile imageFile)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while editing venue");
                ModelState.AddModelError("", "An error occurred while saving the venue. Please try again.");
                return View(viewModel);
            }
        }

        [HttpGet]
        [Route("VenueDelete/{venueId}")]
        public async Task<IActionResult> VenueDelete(int venueId)
        {
            try
            {
                var venue = await dbContext.Venue.FindAsync(venueId);
                if (venue == null)
                {
                    return NotFound();
                }
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving venue for deletion");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the venue. Please try again later." });
            }
        }

        [HttpPost]
        [Route("Delete/{id}")]
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
                _logger.LogError(ex, "Error occurred while deleting venue");
                ModelState.AddModelError("", "An error occurred while deleting the venue. Please try again.");
                return RedirectToAction("VenueList");
            }
        }

        [HttpGet]
        [Route("BlobList")]
        public IActionResult BlobList()
        {
            try
            {
                var blobs = _blobService.ListBlobsAsync().Result;
                return View(blobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving blob list");
                return RedirectToAction("Error", "Home", new { message = "An error occurred while retrieving the blob list. Please try again later." });
            }
        }

        [HttpGet]
        [Route("DownloadImage/{fileName}")]
        public async Task<IActionResult> DownloadImage(string fileName)
        {
            try
            {
                var (stream, contentType) = await _blobService.DownloadAsync(fileName);
                return File(stream, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading image {fileName}", fileName);
                return NotFound();
            }
        }

        [HttpPost]
        [Route("DeleteImage")]
        public async Task<IActionResult> DeleteImage(string fileName, int venueId)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting image {fileName}", fileName);
                return RedirectToAction("Error", "Home", new { message = "An error occurred while deleting the image. Please try again later." });
            }
        }
    }
}
