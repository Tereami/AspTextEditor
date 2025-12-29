using AspTextEditor.Models;
using AspTextEditor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _um;

        public BlogController(Data.DB db, UserManager<AppUser> um)
        {
            _db = db;
            _um = um;
        }

        public async Task<IActionResult> Index()
        {
            Dictionary<string, string> pages = await _db.BlogPages
                .OrderByDescending(p => p.CreatedAt)
                .ToDictionaryAsync(i => i.Slug, j => j.Title);
            return View(pages);
        }

        [HttpGet("Blog/Page/{slug}")]
        public async Task<IActionResult> Page(string slug)
        {
            BlogPage? page = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null) return NotFound();
            BlogPageViewModel viewModel = new BlogPageViewModel(page);
            return View(viewModel);
        }


        [Authorize]
        public IActionResult Create() => View("Edit", new BlogPageViewModel());

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogPageViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            model.CreatedAt = DateTime.UtcNow;
            AppUser? author = await _um.GetUserAsync(HttpContext.User);
            if (author == null)
                return BadRequest("NO SUCH USER");
            BlogPage newPage = new BlogPage
            {
                Title = model.Title,
                Slug = model.Slug,
                HtmlContent = model.HtmlContent,
                Author = author,
                CreatedAt = DateTime.UtcNow,
            };

            _db.BlogPages.Add(newPage);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Edit(string slug)
        {
            BlogPage? page = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Slug == slug);
            if (page == null) return NotFound();
            BlogPageViewModel viewModel = new BlogPageViewModel(page);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BlogPageViewModel model)
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
