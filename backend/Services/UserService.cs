using short_url.Entities;
using short_url.Helpers;
using short_url.Models.Users;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using short_url.Models;
using Microsoft.Extensions.Options;
using short_url.Authorization;

namespace short_url.Services;
public interface IUserService
{
    AuthenticateResponse Authenticate(AuthenticateRequest model);
    IEnumerable<User> GetAll();
    User GetById(Guid id);
    void Register(RegisterRequest model);
    void Update(Guid id, UpdateRequest model);
    void Delete(Guid id);
}
public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;

    public UserService(DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model)
    {
        var user = _context.Users.SingleOrDefault(x => x.Username == model.Username);
        // validate
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        // authentication successful so generate jwt token
        var token = _jwtUtils.GenerateToken(user);

        return new AuthenticateResponse(user, token);
    }



    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(Guid id)
    {
        return getUser(id);
    }

    public void Register(RegisterRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Username == model.Username))
            throw new AppException("User with the username '" + model.Username + "' already exists");

        // map model to new user object
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Username = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
        };
        
        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(Guid id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Email != user.Username && _context.Users.Any(x => x.Username == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // copy model to user and save
        user.LastName = model.LastName;
        user.FirstName = model.FirstName;

        _context.Users.Update(user);
        _context.SaveChanges();
    }

    public void Delete(Guid id)
    {
        var user = getUser(id);
        _context.Users.Remove(user);
        _context.SaveChanges();
    }

    private User getUser(Guid id)
    {
        var user = _context.Users.Find(id);
        if (user == null) throw new KeyNotFoundException("User not found");
        return user;
    }

    private string generateJwtToken(User user)
    {
        // generate token that is valid for 7 days
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}