using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskTracker.Data;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        //public async Task<IActionResult> Index()
        //{
        //      return _context.DashboardItems != null ? 
        //                  View(await _context.DashboardItems.ToListAsync()) :
        //                  Problem("Entity set 'ApplicationDbContext.DashboardItems'  is null.");
        //}

        // GET: Dashboard/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DashboardItems == null)
            {
                return NotFound();
            }

            var dashboardModel = await _context.DashboardItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dashboardModel == null)
            {
                return NotFound();
            }

            return View(dashboardModel);
        }

        // GET: Dashboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dashboard/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,AssignedTo,Status")] DashboardModel dashboardModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dashboardModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dashboardModel);
        }

        // GET: Dashboard/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DashboardItems == null)
            {
                return NotFound();
            }

            var dashboardModel = await _context.DashboardItems.FindAsync(id);
            if (dashboardModel == null)
            {
                return NotFound();
            }
            return View(dashboardModel);
        }

        // POST: Dashboard/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,AssignedTo,Status")] DashboardModel dashboardModel)
        {
            if (id != dashboardModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dashboardModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DashboardModelExists(dashboardModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dashboardModel);
        }

        // GET: Dashboard/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DashboardItems == null)
            {
                return NotFound();
            }

            var dashboardModel = await _context.DashboardItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dashboardModel == null)
            {
                return NotFound();
            }

            return View(dashboardModel);
        }

        // POST: Dashboard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DashboardItems == null)
            {
                return Problem("Entity set 'ApplicationDbContext.DashboardItems'  is null.");
            }
            var dashboardModel = await _context.DashboardItems.FindAsync(id);
            if (dashboardModel != null)
            {
                _context.DashboardItems.Remove(dashboardModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DashboardModelExists(int id)
        {
          return (_context.DashboardItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> Index(string searchString, string statusFilter, DateTime? dueDateFilter)
        {
            var tasks = from t in _context.DashboardItems
                        select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                tasks = tasks.Where(t => t.Title.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(statusFilter))
            {
                tasks = tasks.Where(t => t.Status == statusFilter);
            }

            if (dueDateFilter.HasValue)
            {
                tasks = tasks.Where(t => t.DueDate.Date == dueDateFilter.Value.Date);
            }

            var distinctStatuses = await _context.DashboardItems
                                    .Select(t => t.Status)
                                    .Distinct()
                                    .ToListAsync();

            ViewBag.StatusList = new SelectList(distinctStatuses);

            return View(await tasks.ToListAsync());
        }
    }
}
