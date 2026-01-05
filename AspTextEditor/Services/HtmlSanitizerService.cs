using Ganss.Xss;

namespace AspTextEditor.Services
{
    public class HtmlSanitizerService
    {
        private readonly HtmlSanitizer _sanitizer;

        public HtmlSanitizerService()
        {
            _sanitizer = new HtmlSanitizer();

            _sanitizer.AllowedTags.Add("img");
            _sanitizer.AllowedTags.Add("h1");
            _sanitizer.AllowedTags.Add("h2");
            _sanitizer.AllowedTags.Add("h3");
            _sanitizer.AllowedTags.Add("h4");
            _sanitizer.AllowedTags.Add("h5");
            _sanitizer.AllowedTags.Add("h6");

            _sanitizer.AllowedAttributes.Add("src");
            _sanitizer.AllowedAttributes.Add("alt");
            _sanitizer.AllowedAttributes.Add("class");
        }

        public string Sanitize(string html)
        {
            string sanitizedHtml = _sanitizer.Sanitize(html);
            return sanitizedHtml;
        }
    }
}
