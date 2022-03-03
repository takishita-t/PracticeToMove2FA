using MailKit.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MimeKit;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace PracticeToMove2FA.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> userManager;

        public RegisterModel (UserManager<IdentityUser> userManager)
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

            //validateing Email address

            //Create the userÅ@ìoò^éûÇÃê›íË
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
                return Page();
            }
            else
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage ="Invaild email address.")]
        public string Email { get; set; } = "test@test.com";

        [Required]
        [DataType(dataType:DataType.Password)]
        public string Password { get; set; } = "Password@01";
    }
}
