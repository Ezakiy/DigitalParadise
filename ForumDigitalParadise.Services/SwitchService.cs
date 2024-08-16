using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ForumDigitalParadise.Service
{
    public class SwitchService : ISwitch
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public SwitchService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<bool> SwitchAccountAsync(string currentUserId, string accountId)
        {
            var newUser = await _userManager.FindByIdAsync(accountId);
            if (newUser == null)
                return false;

            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            return true;
        }

        public async Task<List<ApplicationUser>> GetUserAccountsAsync(string userId)
        {
            var accounts = _context.UserAccounts
                                   .Where(ua => ua.PrimaryUserId == userId)
                                   .Select(ua => ua.AccountUser)
                                   .ToList();
            return accounts;
        }

        public async Task<bool> AddUserAccountAsync(string primaryUserId, string newAccountUsername, string newAccountPassword)
        {
            var newUser = await _userManager.FindByNameAsync(newAccountUsername);
            if (newUser == null)
                throw new InvalidOperationException($"User with username '{newAccountUsername}' not found.");

            // Check if the password is correct
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(newUser, newAccountPassword, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded)
                throw new InvalidOperationException("Invalid password.");

            var userAccount = new UserAccount
            {
                PrimaryUserId = primaryUserId,
                AccountUserId = newUser.Id
            };

            _context.UserAccounts.Add(userAccount);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
