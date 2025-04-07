﻿// Author: Microsoft Corporation  
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

namespace ST10435077___CLDV6211_POE.Controllers
{
    public class EventController : Controller
    {
        private readonly EventEaseContext _dbContext;

        public EventController(EventEaseContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult EventAdd()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EventAdd(Event viewModel)
        {
            if (ModelState.IsValid)
            {
                var @event = new Event
                {
                    EventName = viewModel.EventName,
                    EventDate = viewModel.EventDate,
                    Description = viewModel.Description,
                    VenueId = viewModel.VenueId
                };

                await _dbContext.Event.AddAsync(@event);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("EventList");
            }
            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> EventList()
        {
            var events = await _dbContext.Event
                .Include(e => e.Venue)  // Include venue information
                .ToListAsync();
            return View(events);
        }

        [HttpGet]
        public async Task<IActionResult> EventEdit(int eventId)
        {
            var @event = await _dbContext.Event
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(e => e.EventId == eventId);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost]
        public async Task<IActionResult> EventEdit(Event viewModel)
        {
            if (ModelState.IsValid)
            {
                var @event = await _dbContext.Event.FindAsync(viewModel.EventId);
                if (@event != null)
                {
                    @event.EventName = viewModel.EventName;
                    @event.EventDate = viewModel.EventDate;
                    @event.Description = viewModel.Description;
                    @event.VenueId = viewModel.VenueId;

                    await _dbContext.SaveChangesAsync();
                }
                return RedirectToAction("EventList");
            }
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int eventId)
        {
            var @event = await _dbContext.Event.FindAsync(eventId);
            if (@event != null)
            {
                _dbContext.Event.Remove(@event);
                await _dbContext.SaveChangesAsync();
            }
            return RedirectToAction("EventList");
        }
    }
}