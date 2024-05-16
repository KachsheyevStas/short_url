using short_url.Entities;
using short_url.Helpers;

namespace short_url.Services
{
    public class UrlShortenerService
    {
        private DataContext _context;

        public UrlShortenerService(DataContext context)
        {
            _context = context;
        }

        public string ShortenUrl(string originalUrl)
        {
            var mapping = new UrlMapping
            {
                Id = Guid.NewGuid(),
                OriginalUrl = originalUrl,
                ShortUrl = GenerateShortCode(),
                CountClick = 0,
                CreateDate = DateTime.Now,
                
            };

            _context.UrlMappings.Add(mapping);
            _context.SaveChanges();

            return mapping.ShortUrl;
        }

        public string? GetOriginalUrl(string shortCode)
        {
            var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortUrl == shortCode);

            return mapping?.OriginalUrl;
        }

        private string GenerateShortCode()
        {
            return "123213";
        }
    }
}
