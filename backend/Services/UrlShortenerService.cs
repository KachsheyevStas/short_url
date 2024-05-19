using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
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
    string GetOriginalUrl(string shortUrl);
    void Delete(Guid id);
    Task<string> GetDestinationUrlAsync(string shortCode, CancellationToken cancellationToken);
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
        return _context.UrlMappings.Where(t => t.UserId == userId);
    }

    public UrlMapping? GetById(Guid id) => _context.UrlMappings.FirstOrDefault(t => t.Id == id);
    public string? GetOriginalUrl(string shortUrl)
    {
        var mapping = _context.UrlMappings.FirstOrDefault(x => x.ShortUrl == shortUrl);
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

    public async Task<string> GetDestinationUrlAsync(string shortCode, CancellationToken cancellationToken)
    {
        var link = await _context.UrlMappings.FirstOrDefaultAsync(x => x.ShortUrl == shortCode, cancellationToken);

        if (link is not null)
        {
            link.CountClick += 1;
            _context.SaveChanges();
            return link.OriginalUrl;
        }

        throw new Exception("Invalid shorten code!");
    }
    private string GenerateShortCode(Guid id, string scheme, HostString host)
    {
        var urlChunk = WebEncoders.Base64UrlEncode(id.ToByteArray());
        return urlChunk;
        //return $"{scheme}://{host}/{urlChunk}";
    }
}
