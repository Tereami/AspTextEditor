using System;
using System.ComponentModel.DataAnnotations;

namespace AspTextEditor.Models
{
    public class BlogPage
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

        public AppUser Author { get; set; }
    }
}
