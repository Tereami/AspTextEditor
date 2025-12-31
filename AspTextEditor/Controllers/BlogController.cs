using AspTextEditor.Models;
using AspTextEditor.Services;
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
        private readonly HtmlSanitizerService _sanitizer;
        private readonly ImageCleanerService _cleaner;

        public BlogController(Data.DB db, UserManager<AppUser> um, HtmlSanitizerService hss, ImageCleanerService cleaner)
        {
            _db = db;
            _um = um;
            _sanitizer = hss;
            _cleaner = cleaner;
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
                .Include(p => p.Author)
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
                Id = model.Id,
                Title = model.Title,
                Slug = model.Slug,
                HtmlContent = _sanitizer.Sanitize(model.HtmlContent),
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
        public async Task<IActionResult> Edit(BlogPageViewModel editedPage)
        {
            if (!ModelState.IsValid)
                return View(editedPage);

            BlogPage? existedPage = await _db.BlogPages
                .FirstOrDefaultAsync(p => p.Slug == editedPage.Slug);
            if (existedPage == null) return NotFound();

            _cleaner.RemoveUnusedImages(existedPage.HtmlContent, editedPage.HtmlContent);

            existedPage.Title = editedPage.Title;
            existedPage.Slug = editedPage.Slug;
            existedPage.HtmlContent = _sanitizer.Sanitize(editedPage.HtmlContent);
            existedPage.LastUpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return RedirectToAction("Page", new { slug = existedPage.Slug });
        }
    }
}
