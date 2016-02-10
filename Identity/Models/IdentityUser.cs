using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;

namespace CreativeColon.Raven.Identity.Models
{
    public class IdentityUser<TKey, TLogin, TClaim> : IUser<TKey>
        where TLogin : IdentityUserLogin
        where TClaim : IdentityUserClaim
    {
        public virtual TKey Id { get; set; }
        public virtual string UserName { get; set; }

        public virtual string Email { get; set; }
        public virtual bool EmailConfirmed { get; set; }
        public virtual string PasswordHash { get; set; }
        public virtual string SecurityStamp { get; set; }
        public virtual string PhoneNumber { get; set; }
        public virtual bool PhoneNumberConfirmed { get; set; }
        public virtual bool TwoFactorEnabled { get; set; }
        public virtual DateTime? LockoutEndDateUtc { get; set; }
        public virtual bool LockoutEnabled { get; set; }
        public virtual int AccessFailedCount { get; set; }

        public virtual ICollection<string> Roles { get; private set; }
        public virtual ICollection<TClaim> Claims { get; private set; }
        public virtual ICollection<TLogin> Logins { get; private set; }

        public IdentityUser()
        {
            Claims = new List<TClaim>();
            Roles = new List<string>();
            Logins = new List<TLogin>();
        }
    }

    public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityUserClaim>, IUser, IUser<string>
    {
        public IdentityUser()
        {
        }

        public IdentityUser(string userName)
        {
            UserName = userName;
        }
    }
}
