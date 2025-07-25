using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class Province
    {
        public Guid ProvinceID { get; set; }

        public int ProvinceCode { get; set; }
        public string ProvinceName { get; set; } = null!;

        public int noofdistricts { get; set; }

        public int CountryID { get; set; }
        public Country Country { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}