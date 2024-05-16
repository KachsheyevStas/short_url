using short_url.Entities;
using short_url.Helpers;
using BCrypt.Net;
using short_url.Models.Users;

namespace short_url.Services;
public interface IUserService
{
    IEnumerable<User> GetAll();
    User GetById(Guid id);
    void Create(CreateRequest model);
    void Update(Guid id, UpdateRequest model);
    void Delete(Guid id);
}
public class UserService : IUserService
{
    private DataContext _context;

    public UserService(DataContext context)
    {
        _context = context;
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }

    public User GetById(Guid id)
    {
        return getUser(id);
    }

    public void Create(CreateRequest model)
    {
        // validate
        if (_context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // map model to new user object
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Title = model.Title,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role = Role.User,
        };

        // save user
        _context.Users.Add(user);
        _context.SaveChanges();
    }

    public void Update(Guid id, UpdateRequest model)
    {
        var user = getUser(id);

        // validate
        if (model.Email != user.Email && _context.Users.Any(x => x.Email == model.Email))
            throw new AppException("User with the email '" + model.Email + "' already exists");

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // copy model to user and save
        user.Title = model.Title;
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
}