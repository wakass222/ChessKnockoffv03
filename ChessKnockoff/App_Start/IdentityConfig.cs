using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using ChessKnockoff.Models;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using static ChessKnockoff.ApplicationUserManager;
using System.Linq;
using System.Collections.Generic;

namespace ChessKnockoff
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            //Create mail message object
            using (MailMessage mail = new MailMessage())
            {
                //Set the sending email address
                mail.From = new MailAddress(ConfigurationManager.AppSettings["emailServiceUserName"]);
                //Set the destination address
                mail.To.Add(message.Destination);
                //Set the subject
                mail.Subject = message.Subject;
                //Set the body
                mail.Body = message.Body;
                //Make sure the body is rendered as HTML
                mail.IsBodyHtml = true;

                //Create a SmtpClient to gmail servers and the dispose when finished
                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    //Credentials for verification
                    smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["emailServiceUserName"], ConfigurationManager.AppSettings["emailServicePassword"]);
                    //SSL must be enabled
                    smtp.EnableSsl = true;
                    //Send the email
                    await smtp.SendMailAsync(mail);
                }
            }
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    //Create a custom password validator to extend the identity validator
    public class CustomPasswordValidator : PasswordValidator
    {
        //Create a property for the maximum length
        public int MaxLength { get; set; }

        //Override the method to validate passwords
        public override async Task<IdentityResult> ValidateAsync(string item)
        {
            //Call the original validation method and wait for its result
            IdentityResult result = await base.ValidateAsync(item);

            //Get the errors from the original validation method
            List<string> errors = result.Errors.ToList();

            //Check if it is null and if it is larger than the maximum length
            if (string.IsNullOrEmpty(item) || item.Length > MaxLength)
            {
                //Check if there is already an error
                if (errors.FirstOrDefault<string>() != null)
                {
                    //Add the error to the string since the errors only appear as one string
                    errors[0] += string.Format(" Password length can't exceed {0}.", MaxLength);
                } else
                {
                    //Else create a new element using the string format to also write the maximum length
                    errors.Add(string.Format("Password length can't exceed {0}.", MaxLength));
                }
            }
            
            //Wait for the result then return it
            return await Task.FromResult(!errors.Any()
             ? IdentityResult.Success
             : IdentityResult.Failed(errors.ToArray()));
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            ApplicationUserManager manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));

            // Configure validation setting for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = true,
                RequireUniqueEmail = true
                //Username length is done as a custom validation so it can be done client side
            };

            // Configure validation settings for password
            manager.PasswordValidator = new CustomPasswordValidator
            {
                MaxLength = 256,
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is {0}"
            });

            // Configure user lockout defaults
            //Allow the user to be locked out
            manager.UserLockoutEnabledByDefault = true;
            //Set a lockout time
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(30);
            //Set the amount of attempts before a lockout is set
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            manager.EmailService = new ChessKnockoff.EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"))
                {
                    //Set an expiry on the tokens generated including confirmation and reset tokens
                    TokenLifespan = TimeSpan.FromHours(2)
                };
            }
            return manager;
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
