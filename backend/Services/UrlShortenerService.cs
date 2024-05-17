using short_url.Authorization;
using short_url.Entities;
using short_url.Helpers;
using System.Security.Claims;

namespace short_url.Services;

public interface IUrlShortenerService
{
    void Create(string originalUrl, Guid userId);
    IEnumerable<UrlMapping> GetAll(Guid userId);
    UrlMapping GetById(Guid id);
    string GetOriginalUrl(string shortCode);
    void Delete(Guid id);
}

public class UrlShortenerService : IUrlShortenerService
{
    private DataContext _context;

    public UrlShortenerService( DataContext context)
    {
        _context = context;
    }

    public void Create(string originalUrl, Guid userId)
    {
        var mapping = new UrlMapping
        {
            Id = Guid.NewGuid(),
            OriginalUrl = originalUrl,
            ShortUrl = GenerateShortCode(),
            CountClick = 0,
            CreateDate = DateTime.Now,
            UserId = userId
        };
        _context.UrlMappings.Add(mapping);
        _context.SaveChanges();
    }
    public IEnumerable<UrlMapping> GetAll(Guid userId)
    {
        return _context.UrlMappings;
    }

    public UrlMapping? GetById(Guid id) => _context.UrlMappings.FirstOrDefault(t => t.Id == id);
    public string? GetOriginalUrl(string shortCode)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortUrl == shortCode);
        if (mapping == null)
        {
            mapping.CountClick += 1;
        }
        _context.SaveChanges();
        return mapping?.OriginalUrl;
    }


    public void Delete(Guid id)
    {
        var url = _context.UrlMappings.FirstOrDefault(t => t.Id == id);

        _context.UrlMappings.Remove(url);
        _context.SaveChanges();
    }
    private string GenerateShortCode()
    {
        return "123213";
    }
}
