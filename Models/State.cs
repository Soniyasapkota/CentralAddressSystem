using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
    public class State
    {
        public int StateID { get; set; }
        public string StateName { get; set; } = null!;
        public int CountryID { get; set; }
        public Country Country { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}