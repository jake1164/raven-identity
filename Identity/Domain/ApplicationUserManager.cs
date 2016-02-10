using CreativeColon.Raven.Identity.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Raven.Client;
using System;

namespace CreativeColon.Raven.Identity.Domain
{
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var Manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<IAsyncDocumentSession>()));

            Manager.UserValidator = new UserValidator<ApplicationUser>(Manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            Manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };

            Manager.UserLockoutEnabledByDefault = true;
            Manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            Manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            Manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is {0}"
            });

            Manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>());

            Manager.EmailService = new EmailService();
            Manager.SmsService = new SmsService();

            var DataProtectionProvider = options.DataProtectionProvider;
            if (DataProtectionProvider != null)
                Manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(DataProtectionProvider.Create("RavenIdentity"));

            return Manager;
        }
    }
}
