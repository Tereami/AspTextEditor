using AspTextEditor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspTextEditor.Controllers
{
    public class BlogController : Controller
    {
        private readonly AspTextEditor.Data.DB _db;

        public BlogController(Data.DB db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            List<BlogPage> pages = await _db.BlogPages
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(pages);
        }

        public async Task<IActionResult> Page(string slug)
        {
            BlogPage? page = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null) return NotFound();

            return View(page);
        }


        [Authorize]
        public IActionResult Create() => View();

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPage model)
        {
            if (!ModelState.IsValid)
                return View(model);

            model.CreatedAt = DateTime.UtcNow;
            model.AuthorName = User.Identity!.Name!;

            _db.BlogPages.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(string slug)
        {
            BlogPage? page = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null) return NotFound();

            return View("Create", page);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogPage model)
        {
            if (!ModelState.IsValid)
                return View(model);

            BlogPage? page = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Id == model.Id);
            if (page == null) return NotFound();

            page.Title = model.Title;
            page.Slug = model.Slug;
            page.HtmlContent = model.HtmlContent;
            page.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction("Page", new { id = page.Slug });
        }


    }
}
