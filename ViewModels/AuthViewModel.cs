using System.ComponentModel.DataAnnotations;

namespace CentralAddressSystem.ViewModels
{
    public class AuthViewModel
    {
        public RegisterViewModel Register { get; set; } = new RegisterViewModel();
        public LoginViewModel Login { get; set; } = new LoginViewModel();
    }
}