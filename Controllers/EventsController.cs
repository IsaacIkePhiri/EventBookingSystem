using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseBookingSystem.Data;
using EventEaseBookingSystem.Models;

namespace EventEaseBookingSystem.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        // Advanced filtering for Part 3: event type, venue, date range and availability.
        public async Task<IActionResult> Index(
            string? searchString,
            int? eventTypeId,
            int? venueId,
            DateTime? startDate,
            DateTime? endDate,
            bool? availableOnly)
        {
            var eventsQuery = _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                eventsQuery = eventsQuery.Where(e =>
                    e.EventName.Contains(searchString) ||
                    e.Venue!.Name.Contains(searchString) ||
                    e.Venue.Location.Contains(searchString) ||
                    e.EventType!.Name.Contains(searchString));
            }

            if (eventTypeId.HasValue && eventTypeId.Value > 0)
            {
                eventsQuery = eventsQuery.Where(e => e.EventTypeId == eventTypeId.Value);
            }

            if (venueId.HasValue && venueId.Value > 0)
            {
                eventsQuery = eventsQuery.Where(e => e.VenueId == venueId.Value);
            }

            if (startDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.EndDate.Date >= startDate.Value.Date);
            }

            if (endDate.HasValue)
            {
                eventsQuery = eventsQuery.Where(e => e.StartDate.Date <= endDate.Value.Date);
            }

            if (availableOnly == true)
            {
                eventsQuery = eventsQuery.Where(e => !_context.Bookings.Any(b => b.EventId == e.EventId));
            }

            LoadFilterDropdowns(eventTypeId, venueId);

            ViewData["BookedEventIds"] = await _context.Bookings
                .Select(b => b.EventId)
                .Distinct()
                .ToListAsync();

            ViewData["CurrentSearch"] = searchString;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");
            ViewData["AvailableOnly"] = availableOnly == true;

            return View(await eventsQuery.OrderBy(e => e.StartDate).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,StartDate,EndDate,VenueId,EventTypeId")] Event @event)
        {
            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be before start date.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns(@event.VenueId, @event.EventTypeId);
            return View(@event);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            LoadDropdowns(@event.VenueId, @event.EventTypeId);
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,StartDate,EndDate,VenueId,EventTypeId")] Event @event)
        {
            if (id != @event.EventId) return NotFound();

            if (@event.EndDate < @event.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be before start date.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId)) return NotFound();
                    throw;
                }
            }

            LoadDropdowns(@event.VenueId, @event.EventTypeId);
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Venue)
                .Include(e => e.EventType)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);

            if (hasBookings)
            {
                TempData["Error"] = "Cannot delete event with active bookings.";
                return RedirectToAction(nameof(Index));
            }

            var @event = await _context.Events.FindAsync(id);

            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }

        private void LoadDropdowns(object? selectedVenue = null, object? selectedEventType = null)
        {
            ViewData["VenueId"] = new SelectList(_context.Venues.OrderBy(v => v.Name), "VenueId", "Name", selectedVenue);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes.OrderBy(et => et.Name), "EventTypeId", "Name", selectedEventType);
        }

        private void LoadFilterDropdowns(object? selectedEventType = null, object? selectedVenue = null)
        {
            ViewData["EventTypeFilter"] = new SelectList(_context.EventTypes.OrderBy(et => et.Name), "EventTypeId", "Name", selectedEventType);
            ViewData["VenueFilter"] = new SelectList(_context.Venues.OrderBy(v => v.Name), "VenueId", "Name", selectedVenue);
        }
    }
}
