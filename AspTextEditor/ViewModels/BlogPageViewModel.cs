using AspTextEditor.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace AspTextEditor.ViewModels
{
    public class BlogPageViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string HtmlContent { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        public string? AuthorName { get; set; } = string.Empty;
        public string? AuthorId { get; set; } = string.Empty;


        public BlogPageViewModel() { }

        public BlogPageViewModel(BlogPage page)
        {
            Id = page.Id;
            Slug = page.Slug;
            Title = page.Title;
            HtmlContent = page.HtmlContent;
            CreatedAt = page.CreatedAt;
            LastUpdatedAt = page.LastUpdatedAt;
            AuthorName = page.Author?.UserName;
            AuthorId = page.Author?.Id;
        }
    }
}
