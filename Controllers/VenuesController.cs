using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;
using Azure.Storage.Blobs;

namespace EventEaseBookingSystem.Controllers
{
    public class VenuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VenuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Venues
        public async Task<IActionResult> Index()
        {
            return View(await _context.Venues.ToListAsync());
        }

        // GET: Venues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // GET: Venues/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venues/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Venue venue, IFormFile imageFile)
        {
            try
            {
                // IMAGE UPLOAD
                if (imageFile != null && imageFile.Length > 0)
                {
                    var connectionString =
                        "DefaultEndpointsProtocol=http;" +
                        "AccountName=devstoreaccount1;" +
                        "AccountKey=Eby8vdM02xNOcqFeqCnf2g==;" +
                        "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";

                    var containerName = "venueimages";

                    var containerClient =
                        new BlobContainerClient(connectionString, containerName);

                    await containerClient.CreateIfNotExistsAsync();

                    var blobClient =
                        containerClient.GetBlobClient(imageFile.FileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                if (ModelState.IsValid)
                {
                    _context.Venues.Add(venue);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message + " --- " + ex.InnerException?.Message);
            }

            return View(venue);
        }

        // GET: Venues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues.FindAsync(id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Venue venue, IFormFile imageFile)
        {
            if (id != venue.VenueId)
                return NotFound();

            try
            {
                // IMAGE UPLOAD
                if (imageFile != null && imageFile.Length > 0)
                {
                    var connectionString =
                        "DefaultEndpointsProtocol=http;" +
                        "AccountName=devstoreaccount1;" +
                        "AccountKey=Eby8vdM02xNOcqFeqCnf2g==;" +
                        "BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;";

                    var containerName = "venueimages";

                    var containerClient =
                        new BlobContainerClient(connectionString, containerName);

                    await containerClient.CreateIfNotExistsAsync();

                    var blobClient =
                        containerClient.GetBlobClient(imageFile.FileName);

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, true);
                    }

                    venue.ImageUrl = blobClient.Uri.ToString();
                }

                if (ModelState.IsValid)
                {
                    _context.Update(venue);

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message + " --- " + ex.InnerException?.Message);
            }

            return View(venue);
        }

        // GET: Venues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var venue = await _context.Venues
                .FirstOrDefaultAsync(m => m.VenueId == id);

            if (venue == null)
                return NotFound();

            return View(venue);
        }

        // POST: Venues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var hasBookings = _context.Bookings.Any(b => b.VenueId == id);

                if (hasBookings)
                {
                    TempData["Error"] =
                        "Cannot delete venue with active bookings.";

                    return RedirectToAction(nameof(Index));
                }

                var venue = await _context.Venues.FindAsync(id);

                if (venue != null)
                {
                    _context.Venues.Remove(venue);

                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return Content(ex.Message + " --- " + ex.InnerException?.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(int id)
        {
            return _context.Venues.Any(e => e.VenueId == id);
        }
    }
}