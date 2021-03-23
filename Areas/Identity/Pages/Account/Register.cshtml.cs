using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using test_mvc_webapp.Models;
// using test_mvc_webapp.Helper;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace test_mvc_webapp.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        // This method will be called on registration.
        // We tailored it to automatically add new registrations to the 'user' role
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {

            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                // Original user commented out
                //var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var user = new ApplicationUser // Our new extended user
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName
                };

                // Create the user
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    // Unless I encoded the generated token the link in email would not work
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    
                    var confirmationLink = Url.Action("ConfirmEmail", "Email", new { code, email = user.Email }, Request.Scheme);
                    
                    // Should getapikey from environment or other for security reasons
                    // var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
                    var apiKey ="nptinsourcecontrol";
                    var client = new SendGridClient(apiKey); // Get a mail client reference to send an email
                    // Generate a URL for email that will route properly on the backend
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Build the confirm registration message
                    var msg = new SendGridMessage()
                    {
                        // From email address MUST match your approved sender email in sendgrid
                        From = new EmailAddress("kevin@code-crew.org", "CoolApp Administrators"),
                        Subject = "Please confirm your email",
                        PlainTextContent = $"Hello, {user.FirstName} {user.LastName}!",
                        HtmlContent = $"Please confirm your account by clicking here <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>"
                    };
                    // Add the User as a 'to' address. You could have multiple with some emails which is why we use this method to add
                    msg.AddTo(new EmailAddress(user.Email, $"{user.FirstName} {user.LastName}"));
                    // Send the email
                    var response = await client.SendEmailAsync(msg);
                    // // Attempt to auto-add role for new user to 'user' role
                    await _userManager.AddToRoleAsync(user, "user");
                    // If we require email confirmation, do not sign in new user automatially
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
