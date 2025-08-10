using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class Address
    {
        public int AddressID { get; set; }
        public int UserID { get; set; } // Will reference AspNetUsers.Id
        public User User { get; set; } = null!;
        public int CountryID { get; set; }
        public Country Country { get; set; } = null!;
        public int? ProvinceID { get; set; }
        public Province? Province { get; set; }
        public int? DistrictID { get; set; }
        public District? District { get; set; }
        public Guid? LocalBodyID { get; set; }
        public LocalBody? LocalBody { get; set; }
        public int? StateID { get; set; }
        public State? State { get; set; }
        public int? ZipID { get; set; }
        public ZipCode? ZipCode { get; set; }
        public string Street { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
