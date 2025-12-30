using AngleSharp.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AspTextEditor.Controllers
{
    [Authorize]
    public class UploadController : Controller
    {
        private string uploadsFolder = "uploads";
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }


        [HttpPost]
        public async Task<IActionResult> Image(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file");

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest("File is too large");

            string[] allowedTypes = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedTypes.Contains(ext))
                return BadRequest($"Invalid file type: {ext}");

            string fileName = $"{Guid.NewGuid()}{ext}";
            string uploadFolder = Path.Combine(_env.WebRootPath, uploadsFolder);
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);
            string fullPath = Path.Combine(uploadFolder, fileName);

            await using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Json(new { location = $"/{uploadsFolder}/{fileName}" });
        }
    }
}
