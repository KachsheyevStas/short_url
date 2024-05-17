using System.Globalization;

namespace short_url.Services;
public interface IIdentityService
{
    string Language { get; }

    Guid UserId { get; }

    string UserName { get; }

    bool IsAuthenticated { get; }

    bool IsRussianCulture();

    void SetUiCulture();
}
internal class IdentityService : IIdentityService
{
    public IdentityService()
    {
        
    }

    private Guid? _userId;
    private string _userName;

    public string Language
    {
        get
        {
            var acceptLanguage = _httpContextAccessor.HttpContext.Request.GetTypedHeaders().AcceptLanguage;

            if (acceptLanguage != null)
            {
                return acceptLanguage.OrderByDescending(x => x.Quality).Select(x => x.Value.Value)
                           .FirstOrDefault() ??
                       "en";
            }

            return "en";
        }
    }

    public Guid UserId
    {
        get
        {
            if (!_userId.HasValue)
            {
                var userIdClaim =
                    _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(t => t.Type == "UserId");
                if (userIdClaim != null)
                {
                    _userId = Guid.Parse(userIdClaim.Value);
                }
            }

            if (!_userId.HasValue)
            {
                return Guid.Empty;
            }

            return _userId.Value;
        }
    }

    public bool IsAuthenticated
    {
        get
        {
            return _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(t => t.Type == "UserId") != null;

        }
    }

    public string UserName
    {
        get
        {
            if (string.IsNullOrEmpty(_userName))
            {
                var userIdClaim =
                    _httpContextAccessor.HttpContext.User.Identity.Name;
                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    _userName = userIdClaim;
                }
            }
            if (string.IsNullOrEmpty(_userName))
            {
                return "Unauthorized";
                //    throw new AccessViolationException("Not authorized user");
            }
            return _userName;
        }
    }


    public bool IsRussianCulture()
    {
        return Language == "ru";
    }

    public void SetUiCulture()
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(Language);
    }

    private readonly IHttpContextAccessor _httpContextAccessor;
}
