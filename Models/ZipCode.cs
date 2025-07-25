using Microsoft.AspNetCore.Identity;

namespace CentralAddressSystem.Models
{
   public class ZipCode
    {
        public int ZipID { get; set; } // Primary key matching ZipCodes.ZipID
        public string Code { get; set; } = null!;
        public int StateID { get; set; }
        public State State { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}