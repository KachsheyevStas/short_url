using short_url.Models;
using Microsoft.AspNetCore.Mvc;
using short_url.Models.Users;
using short_url.Services;
using short_url.Helpers;
using System.Security.Claims;
using short_url.Authorization;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace short_url.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlsController : ControllerBase
{
    private IUrlShortenerService _urlShortService;
    private IJwtUtils _jwtUtils;
    public UrlsController(IUrlShortenerService urlShortService, IJwtUtils jwtUtils)
    {
        _urlShortService = urlShortService;
        _jwtUtils = jwtUtils;
    }


    [HttpPost("createLink")]
    public IActionResult Create(string originalUrl)
    {
        try
        {
            if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out var inputUri))
            {
                throw new AppException("URL is invalid");
            }

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host;
            var userId = _jwtUtils.ValidateToken(token);

            _urlShortService.Create(originalUrl, userId.Value,  scheme, host);
            return Ok(new { message = "Link created" });
        }
        catch (AppException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = _jwtUtils.ValidateToken(token);
        var urls = _urlShortService.GetAll(userId.Value);
        return Ok(urls);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(Guid id)
    {
        var url = _urlShortService.GetById(id);
        return Ok(url);
    }

    [HttpPost("getByShortUrl")]
    public IActionResult GetByShortUrl([FromBody] string shortUrl)
    {
        var url = _urlShortService.GetOriginalUrl(shortUrl);
        return Ok(url);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        _urlShortService.Delete(id);
        return Ok(new { message = "Link deleted" });
    }
}