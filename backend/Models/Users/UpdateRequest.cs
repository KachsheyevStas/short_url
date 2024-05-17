using short_url.Entities;
using System.ComponentModel.DataAnnotations;

namespace short_url.Models.Users
{
    public class UpdateRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        // treat empty string as null for password fields to 
        // make them optional in front end apps
        private string _password;
        [MinLength(6)]
        public string Password
        {
            get => _password;
            set => _password = replaceEmptyWithNull(value);
        }

        // helpers

        private string replaceEmptyWithNull(string value)
        {
            // replace empty string with null to make field optional
            return string.IsNullOrEmpty(value) ? null : value;
        }
    }
}
