
using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class Country
    {
        public Guid CountryID { get; set; }
        public string CountryName { get; set; } = null!;
        public string? CountryCode { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}