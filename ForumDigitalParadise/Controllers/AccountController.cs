using ForumDigitalParadise.Areas.Identity.Pages.Account;
using ForumDigitalParadise.Data;
using ForumDigitalParadise.Data.Models;
using ForumDigitalParadise.Models.User;
using ForumDigitalParadise.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserStore<ApplicationUser> _userStore;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly IEmailSender _emailSender;
    private readonly ApplicationDbContext _context;
    private readonly ISwitch _switchService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISwitch switchService,
        ILogger<AccountController> logger)
    {
        _userManager = userManager;
        _userStore = userStore;
        _emailStore = GetEmailStore();
        _signInManager = signInManager;
        _emailSender = emailSender;
        _switchService = switchService;
        _logger = logger;
    }


    [HttpPost]
    public async Task<IActionResult> AddAccount(string newAccountUsername, string newAccountPassword)
    {
        if (string.IsNullOrEmpty(newAccountUsername) || string.IsNullOrEmpty(newAccountPassword))
        {
            return BadRequest("Username and password cannot be empty.");
        }

        string primaryUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        _logger.LogInformation("Adding account for primary user ID: {PrimaryUserId}", primaryUserId);

        try
        {
            _logger.LogInformation("Looking up user with username: {Username}", newAccountUsername);
            var newUser = await _userManager.FindByNameAsync(newAccountUsername);
            if (newUser == null)
            {
                _logger.LogWarning("User with username '{Username}' not found.", newAccountUsername);
                return BadRequest("User not found.");
            }

            _logger.LogInformation("User found. Checking password for user: {Username}", newAccountUsername);
            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(newUser, newAccountPassword, lockoutOnFailure: false);
            if (!passwordCheck.Succeeded)
            {
                _logger.LogWarning("Invalid password for user: {Username}", newAccountUsername);
                return BadRequest("Invalid password.");
            }

            _logger.LogInformation("Password validated. Adding user account link for primary user ID: {PrimaryUserId} to new user ID: {NewUserId}", primaryUserId, newUser.Id);
            await _switchService.AddUserAccountAsync(primaryUserId, newAccountUsername, newAccountPassword);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding account for user: {Username}", newAccountUsername);
            return Json(new { success = false, error = ex.Message });
        }
    }

   [HttpGet]
public async Task<IActionResult> SwitchAccount(string accountId)
{
    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    var result = await _switchService.SwitchAccountAsync(currentUserId, accountId);

    if (result)
    {
        // Retrieve the new account user object
        var newUser = await _userManager.FindByIdAsync(accountId);
        if (newUser != null)
        {
            // Sign out the current user
            await _signInManager.SignOutAsync();

            // Sign in with the new account
            await _signInManager.SignInAsync(newUser, isPersistent: false);

            // Redirect to the homepage or any specific page with a success message
            TempData["SuccessMessage"] = "Account switch successful.";
            return RedirectToAction("Index", "Home");
        }
    }

    // If the switch failed, return a failure message
    TempData["ErrorMessage"] = "Account switch failed.";
    return RedirectToAction("Index", "Home");
}


    [HttpGet]
    public async Task<IActionResult> GetUserAccounts()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var accounts = await _switchService.GetUserAccountsAsync(currentUserId);
        var accountList = accounts.Select(a => new { id = a.Id, username = a.UserName }).ToList();

        return Json(accountList);
    }

    private string GenerateRandomDisplayName()
    {
        var adjectives = new[] { "Cool", "Happy", "Bright", "Swift", "Mighty" };
        var nouns = new[] { "Eagle", "Tiger", "Shark", "Panther", "Falcon" };
        var random = new Random();
        return adjectives[random.Next(adjectives.Length)] + nouns[random.Next(nouns.Length)] + random.Next(1000, 9999);
    }

    private async Task<int> GetNextDisplayIdAsync()
    {
        var lastUser = await _userManager.Users.OrderByDescending(u => u.DisplayId).FirstOrDefaultAsync();
        return (lastUser?.DisplayId ?? 0) + 1;
    }

    [HttpGet]
    public async Task<IActionResult> Register(string returnUrl = null)
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(LoginRegisterViewModel loginRegisterViewModel)
    {
        string returnUrl = Url.Content("~/");
        ModelState.Remove("LoginModel");
        ModelState.Remove("RegisterModel.ReturnUrl");
        ModelState.Remove("RegisterModel.ErrorMessage");
        ModelState.Remove("RegisterModel.ExternalLogins");

        if (ModelState.IsValid)
        {
            var user = CreateUser();
            string randomDisplayName = GenerateRandomDisplayName();

            await _userStore.SetUserNameAsync((ApplicationUser)user, loginRegisterViewModel.RegisterModel.Input.Username, CancellationToken.None);
            await _userStore.SetNormalizedUserNameAsync((ApplicationUser)user, loginRegisterViewModel.RegisterModel.Input.Username.ToUpper(), CancellationToken.None);
            await _emailStore.SetEmailAsync((ApplicationUser)user, loginRegisterViewModel.RegisterModel.Input.Email, CancellationToken.None);

            ((ApplicationUser)user).DisplayName = randomDisplayName;
            ((ApplicationUser)user).DisplayId = await GetNextDisplayIdAsync(); 
            ((ApplicationUser)user).MemberSince = DateTime.Now;

            var result = await _userManager.CreateAsync((ApplicationUser)user, loginRegisterViewModel.RegisterModel.Input.Password);

            if (result.Succeeded)
            {
                var userId = await _userManager.GetUserIdAsync((ApplicationUser)user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync((ApplicationUser)user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = loginRegisterViewModel.RegisterModel.Input.Email, returnUrl = returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync((ApplicationUser)user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        else
        {
            TempData["SuccessMessage"] = "Registration failed.";
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Index", "Home");
    }

    private IdentityUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore()
    {
        if (!_userManager.SupportsUserEmail)
        {
            throw new NotSupportedException("The default UI requires a user store with email support.");
        }
        return (IUserEmailStore<ApplicationUser>)_userStore;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        string returnUrl = null;
        returnUrl ??= Url.Content("~/");

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRegisterViewModel loginRegisterViewModel)
    {
        string returnUrl = Url.Content("~/");

        ModelState.Remove("RegisterModel");
        ModelState.Remove("LoginModel.ReturnUrl");
        ModelState.Remove("LoginModel.ErrorMessage");
        ModelState.Remove("LoginModel.ExternalLogins");
        ModelState.Remove("LoginModel.Input.RememberMe");

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRegisterViewModel.LoginModel.Input.Username, loginRegisterViewModel.LoginModel.Input.Password, loginRegisterViewModel.LoginModel.Input.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginRegisterViewModel.LoginModel.Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                TempData["SuccessMessage"] = "Login failed.";
                return RedirectToAction("Index", "Home");
            }
        }
        return View(loginRegisterViewModel);
    }
}
