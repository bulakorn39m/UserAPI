using System.ComponentModel.DataAnnotations;

namespace UserAPI.Models
{
    public class GeoDo
    {
        [Required]
        public string? Lat { get; set; }
        [Required]
        public string? Lng { get; set; }
    }

    public class AddressDo
    {
        [Required]
        public string? Street { get; set; }
        [Required]
        public string? Suite { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Zipcode { get; set; }
        [Required]
        public GeoDo? Geo { get; set; }
    }

    public class CompanyDo
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? CatchPhrase { get; set; }
        public string? Bs { get; set; }
    }

    public class UserDo
    {
        [Required]
        public long? Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public AddressDo? Address { get; set; }
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Website { get; set; }
        [Required]
        public CompanyDo? Company { get; set; }
    }
}
