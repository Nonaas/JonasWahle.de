using JonasWahle.de.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace JonasWahle.de.Data.Models
{
    public class User : BaseEntity
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihren Benutzernamen ein")]
        public required string Username { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihr Passwort ein")]
        public required string Password { get; set; }

        public required Role Role { get; set; }
    }
}
