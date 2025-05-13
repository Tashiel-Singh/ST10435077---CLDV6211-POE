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
using System.Diagnostics;

namespace ST10435077___CLDV6211_POE.Controllers
{
    public class VenueController : Controller
    {
        private readonly EventEaseContext _dbContext;
        private readonly BlobService _blobService;
        private readonly ILogger<VenueController> _logger;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private const int MaxFileSizeInBytes = 5 * 1024 * 1024; // 5MB

        public VenueController(EventEaseContext dbContext, BlobService blobService, ILogger<VenueController> logger)
        {
            _dbContext = dbContext;
            _blobService = blobService;
            _logger = logger;
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            return await VenueList();
        }

        // GET: Venue/VenueList
        public async Task<IActionResult> VenueList()
        {
            try
            {
                if (_dbContext.Venue == null)
                {
                    _logger.LogError("Venue DbSet is null");
                    return RedirectToAction("Error", "Home", new { message = "Database configuration error" });
                }

                var venues = await _dbContext.Venue
                    .OrderBy(v => v.VenueName)
                    .ToListAsync();

                _logger.LogInformation($"Retrieved {venues.Count} venues successfully");
                return View(venues);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while retrieving venues");
                return RedirectToAction("Error", "Home", new { message = "A database error occurred. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venues");
                return RedirectToAction("Error", "Home", new { message = "Unable to retrieve venues. Please try again." });
            }
        }

        // GET: Venue/VenueAdd
        public IActionResult VenueAdd()
        {
            return View(new Venue());
        }

        // POST: Venue/VenueAdd
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VenueAdd(Venue venue, IFormFile imageFile)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(venue);
                }

                if (imageFile != null)
                {
                    var validationError = ValidateImageFile(imageFile);
                    if (!string.IsNullOrEmpty(validationError))
                    {
                        ModelState.AddModelError("ImageFile", validationError);
                        return View(venue);
                    }

                    try
                    {
                        venue.ImageUrl = await _blobService.UploadAsync(imageFile);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading image to blob storage");
                        ModelState.AddModelError("ImageFile", "Failed to upload image. Please try again.");
                        return View(venue);
                    }
                }

                _dbContext.Add(venue);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Successfully added venue: {venue.VenueName}");
                return RedirectToAction(nameof(VenueList));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while adding venue");
                ModelState.AddModelError("", "Failed to save venue. Please try again.");
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding venue");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(venue);
            }
        }

        private string ValidateImageFile(IFormFile file)
        {
            if (file.Length > MaxFileSizeInBytes)
            {
                return $"File size must be less than {MaxFileSizeInBytes / 1024 / 1024}MB";
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
            {
                return $"File type not allowed. Allowed types: {string.Join(", ", _allowedExtensions)}";
            }

            return string.Empty;
        }

        // GET: Venue/VenueEdit/{VenueID}
        public async Task<IActionResult> VenueEdit(int VenueID)
        {
            try
            {
                var venue = await _dbContext.Venue.FindAsync(VenueID);
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venue for editing");
                return RedirectToAction("Error", "Home", new { message = "Unable to retrieve venue for editing. Please try again." });
            }
        }

        // POST: Venue/VenueEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VenueEdit(Venue venue, IFormFile imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingVenue = await _dbContext.Venue.FindAsync(venue.VenueId);

                    if (existingVenue != null)
                    {
                        existingVenue.VenueName = venue.VenueName;
                        existingVenue.Location = venue.Location;
                        existingVenue.Capacity = venue.Capacity;

                        if (imageFile != null)
                        {
                            var validationError = ValidateImageFile(imageFile);
                            if (!string.IsNullOrEmpty(validationError))
                            {
                                ModelState.AddModelError("ImageFile", validationError);
                                return View(venue);
                            }

                            try
                            {
                                existingVenue.ImageUrl = await _blobService.UploadAsync(imageFile);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error uploading image to blob storage");
                                ModelState.AddModelError("ImageFile", "Failed to upload image. Please try again.");
                                return View(venue);
                            }
                        }

                        await _dbContext.SaveChangesAsync();
                        _logger.LogInformation($"Successfully edited venue: {venue.VenueName}");
                        return RedirectToAction(nameof(VenueList));
                    }
                }
                return View(venue);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while editing venue");
                ModelState.AddModelError("", "Failed to save venue changes. Please try again.");
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing venue");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return View(venue);
            }
        }

        // GET: Venue/VenueDelete/{venueId}
        public async Task<IActionResult> VenueDelete(int venueId)
        {
            try
            {
                var venue = await _dbContext.Venue.FindAsync(venueId);
                if (venue == null)
                {
                    return NotFound();
                }
                return View(venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving venue for deletion");
                return RedirectToAction("Error", "Home", new { message = "Unable to retrieve venue for deletion. Please try again." });
            }
        }

        // POST: Venue/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var venue = _dbContext.Venue.Include(v => v.Bookings).FirstOrDefault(v => v.VenueId == id);

                if (venue == null)
                {
                    return NotFound();
                }

                if (venue.Bookings.Any())
                {
                    ModelState.AddModelError("", "This venue cannot be deleted as it is associated with active bookings.");
                    return RedirectToAction(nameof(VenueList));
                }

                _dbContext.Venue.Remove(venue);
                _dbContext.SaveChanges();
                _logger.LogInformation($"Successfully deleted venue: {venue.VenueName}");
                return RedirectToAction(nameof(VenueList));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while deleting venue");
                ModelState.AddModelError("", "Failed to delete venue. Please try again.");
                return RedirectToAction(nameof(VenueList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting venue");
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                return RedirectToAction(nameof(VenueList));
            }
        }

        // GET: Venue/BlobList
        public IActionResult BlobList()
        {
            try
            {
                var blobs = _blobService.ListBlobsAsync().Result;
                return View(blobs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blob list");
                return RedirectToAction("Error", "Home", new { message = "Unable to retrieve blob list. Please try again." });
            }
        }

        // GET: Venue/DownloadImage/{fileName}
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

        // POST: Venue/DeleteImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(string fileName, int venueId)
        {
            try
            {
                await _blobService.DeleteAsync(fileName);
                var venue = await _dbContext.Venue.FindAsync(venueId);
                if (venue != null && venue.ImageUrl == fileName)
                {
                    venue.ImageUrl = null;
                    await _dbContext.SaveChangesAsync();
                }
                _logger.LogInformation($"Successfully deleted image: {fileName}");
                return RedirectToAction(nameof(VenueEdit), new { VenueID = venueId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image {fileName}", fileName);
                return RedirectToAction("Error", "Home", new { message = "Unable to delete image. Please try again." });
            }
        }
    }
}
