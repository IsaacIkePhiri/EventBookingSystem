using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;

namespace EventEaseBookingSystem.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue);

            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            try
            {
                // 🔥 FORCE VALIDATION CHECK
                if (booking.VenueId == 0 || booking.EventId == 0)
                {
                    ModelState.AddModelError("", "Please select both Venue and Event.");
                }

                // 🚫 DOUBLE BOOKING CHECK
                var exists = await _context.Bookings.AnyAsync(b =>
                    b.VenueId == booking.VenueId &&
                    b.BookingDate.Date == booking.BookingDate.Date
                );

                if (exists)
                {
                    ModelState.AddModelError("", "This venue is already booked on that date.");
                }

                if (ModelState.IsValid)
                {
                    _context.Bookings.Add(booking);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                // 🔥 SHOW REAL ERROR IN BROWSER
                return Content("ERROR: " + ex.Message + " | " + ex.InnerException?.Message);
            }

            LoadDropdowns(booking);
            return View(booking);
        }

        // GET: Bookings/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // 🔍 SEARCH
        public async Task<IActionResult> Search(string query)
        {
            var bookings = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                bookings = bookings.Where(b =>
                    b.Event!.EventName.Contains(query) ||
                    b.Event.EventType!.Name.Contains(query) ||
                    b.Venue!.Name.Contains(query) ||
                    b.BookingId.ToString().Contains(query));
            }

            return View("Index", await bookings.ToListAsync());
        }

        // 🔧 LOAD DROPDOWNS
        private void LoadDropdowns(Booking booking = null)
        {
            ViewData["VenueId"] = new SelectList(
                _context.Venues,
                "VenueId",
                "Name",
                booking?.VenueId
            );

            ViewData["EventId"] = new SelectList(
                _context.Events,
                "EventId",
                "EventName",
                booking?.EventId
            );
        }
    }
}