using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AspTextEditor.Services
{
    public class ImageCleanerService
    {
        private readonly IWebHostEnvironment _env;
        private readonly string uploadsFolder = string.Empty;

        public ImageCleanerService(IWebHostEnvironment env, IConfiguration conf)
        {
            _env = env;
            uploadsFolder = conf["UploadedImagesPath"] ?? throw new Exception($"Invalid config file");
        }

        public void RemoveUnusedImages(string oldHtml, string newHtml)
        {
            HashSet<string> oldImages = ExtractImages(oldHtml);
            HashSet<string> newImages = ExtractImages(newHtml);

            List<string> removedImages = oldImages.Except(newImages).ToList();

            foreach (string img in removedImages)
            {
                string fullPath = Path.Combine(_env.WebRootPath, img.TrimStart('/'));

                if (File.Exists(fullPath))
                    File.Delete(fullPath);
            }
        }

        private HashSet<string> ExtractImages(string html)
        {
            HashSet<string> images = new HashSet<string>();

            if (string.IsNullOrWhiteSpace(html))
                return images;

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//img[@src]");
            if (nodes == null || nodes.Count == 0)
                return images;

            foreach (HtmlNode node in nodes)
            {
                string src = node.GetAttributeValue("src", "");

                if (src.StartsWith($"/{uploadsFolder}/"))
                    images.Add(src);
            }

            return images;
        }
    }
}
