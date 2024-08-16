using System.ComponentModel.DataAnnotations;
using ForumDigitalParadise.Areas.Identity.Pages.Account;

namespace ForumDigitalParadise.Models.User
{
    public class LoginRegisterViewModel
    {
        public LoginModel LoginModel { get; set; }

        public RegisterModel RegisterModel { get; set; }
    }
}
