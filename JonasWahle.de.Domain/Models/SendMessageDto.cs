using System.ComponentModel.DataAnnotations;

namespace JonasWahle.de.Domain.Models
{
    public class SendMessageDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihren Namen an")]
        [StringLength(maximumLength: 50, MinimumLength = 2, ErrorMessage = "Mindestens 2 und maximal 50 Zeichen erlaubt")]
        public string? Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihre Email an")]
        [EmailAddress(ErrorMessage = "Bitte geben Sie eine gültige Email-Adresse ein")]
        [StringLength(maximumLength: 100, MinimumLength = 5, ErrorMessage = "Mindestens 5 und maximal 100 Zeichen erlaubt")]
        public string? Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Bitte geben Sie Ihre Nachricht an")]
        [StringLength(maximumLength: 1000, MinimumLength = 5, ErrorMessage = "Mindestens 5 und maximal 1000 Zeichen erlaubt")]
        public string? Message { get; set; }
    }
}
