using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using MimeKit;
using MailKit.Security;

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
            //if (ModelState.IsValid) return Page();

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

                //send mail
                var emailMessage = new MimeMessage();

                emailMessage.From.Add(new MailboxAddress("", "test@example.com"));

                emailMessage.To.Add(new MailboxAddress("test@test.com", "test@test.com"));

                emailMessage.Subject = "Confirm your email";

                emailMessage.Body = new TextPart("plain") { Text = $"Confirm your email {confirmationLink}" };

                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    await client.ConnectAsync("localhost", 1025, SecureSocketOptions.Auto);
                    //await client.AuthenticateAsync(_sendMailParams.User, _sendMailParams.Password);
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
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
            }
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invaild email address.")]
        public string Email { get; set; } = "test@test.com";

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = "Password@01";
    }
}
