using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;

namespace EventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public VenuesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.OrderBy(v => v.Name).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageUrl = await UploadImageAsync(imageFile);
                }

                _context.Venues.Add(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FindAsync(id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile? imageFile)
        {
            if (id != venue.VenueId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingVenue = await _context.Venues.AsNoTracking().FirstOrDefaultAsync(v => v.VenueId == id);
                if (existingVenue == null) return NotFound();

                if (imageFile != null && imageFile.Length > 0)
                {
                    venue.ImageUrl = await UploadImageAsync(imageFile);
                }
                else
                {
                    venue.ImageUrl = existingVenue.ImageUrl;
                }

                _context.Update(venue);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(venue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var venue = await _context.Venues.FirstOrDefaultAsync(m => m.VenueId == id);
            if (venue == null) return NotFound();

            return View(venue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasBookings = await _context.Bookings.AnyAsync(b => b.VenueId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete venue with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var venue = await _context.Venues.FindAsync(id);

            if (venue != null)
            {
                _context.Venues.Remove(venue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var connectionString = _configuration.GetConnectionString("AzureStorage")
                ?? _configuration["AzureStorage:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Azure Storage connection string is missing.");
            }

            var containerName = _configuration["AzureStorage:ContainerName"] ?? "venueimages";
            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var extension = Path.GetExtension(imageFile.FileName);
            var blobName = $"venues/{Guid.NewGuid()}{extension}";
            var blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = imageFile.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}
