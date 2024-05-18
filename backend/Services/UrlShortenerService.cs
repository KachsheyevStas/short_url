using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using short_url.Authorization;
using short_url.Entities;
using short_url.Helpers;
using System.Net.Http;
using System.Security.Claims;

namespace short_url.Services;

public interface IUrlShortenerService
{
    void Create(string originalUrl, Guid userId, string scheme, HostString host);
    IEnumerable<UrlMapping> GetAll(Guid userId);
    UrlMapping GetById(Guid id);
    string GetOriginalUrl(Guid id);
    void Delete(Guid id);
}

public class UrlShortenerService : IUrlShortenerService
{
    private DataContext _context;

    public UrlShortenerService( DataContext context)
    {
        _context = context;
    }

    public void Create(string originalUrl, Guid userId, string scheme, HostString host)
    {   
        var mapping = new UrlMapping
        {
            Id = Guid.NewGuid(),
            OriginalUrl = originalUrl,
            CountClick = 0,
            CreateDate = DateTime.Now,
            UserId = userId
        };
        mapping.ShortUrl = GenerateShortCode(mapping.Id, scheme, host);

        _context.UrlMappings.Add(mapping);
        _context.SaveChanges();
    }
    public IEnumerable<UrlMapping> GetAll(Guid userId)
    {
        return _context.UrlMappings;
    }

    public UrlMapping? GetById(Guid id) => _context.UrlMappings.FirstOrDefault(t => t.Id == id);
    public string? GetOriginalUrl(Guid id)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.Id == id);
        if (mapping != null)
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
    private string GenerateShortCode(Guid id, string scheme, HostString host)
    {
        var urlChunk = WebEncoders.Base64UrlEncode(id.ToByteArray());
        return $"{scheme}://{host}/{urlChunk}";
    }
}
