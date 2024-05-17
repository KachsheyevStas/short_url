using short_url.Entities;
using System.ComponentModel.DataAnnotations;

namespace short_url.Models.Users;

public class RegisterRequest
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }
}