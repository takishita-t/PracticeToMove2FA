using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace PracticeToMove2FA.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ResisterController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        public ResisterController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid) return Ok();

            //validateing Email address

            //Create the user　登録時の設定
            var user = new IdentityUser
            {
                Email = RegisterViewModel.Email,
                UserName = RegisterViewModel.Email,
                TwoFactorEnabled = true
            };

            var result = await this.userManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                var confirmationToken = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { userId = user.Id, token = confirmationToken });

                //Send Mail
                var message = new MailMessage(
                    "0209takumi.t@gmail.com",
                    user.Email,
                    "Please confirm your email",
                    $"Please click on this link to confirm your email address: { confirmationToken }");
                using (var emailClient = new SmtpClient("smtp-relay.sendinblue.com", 587))
                {
                    emailClient.Credentials = new NetworkCredential(
                    "0209takumi.t@gmail.com",
                    "aVnLsqZwOJIjx5GU");

                    await emailClient.SendMailAsync(message);
                }
                //return RedirectToPage("/Account/Login");
                return Ok("ok");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Ok("NG");
            }
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invaild email address.")]
        public string Email { get; set; } = "0209takumi.t@gmail.com";

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = "Password@01";
    }
}
