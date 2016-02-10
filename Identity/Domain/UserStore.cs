using CreativeColon.Raven.Identity.Models;
using Microsoft.AspNet.Identity;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CreativeColon.Raven.Identity.Domain
{
    public class UserStore<TUser, TKey, TUserLogin, TUserClaim> : IQueryableUserStore<TUser, TKey>, IUserLoginStore<TUser, TKey>, IUserClaimStore<TUser, TKey>, IUserRoleStore<TUser, TKey>, IUserPasswordStore<TUser, TKey>, IUserSecurityStampStore<TUser, TKey>, IUserEmailStore<TUser, TKey>, IUserPhoneNumberStore<TUser, TKey>, IUserTwoFactorStore<TUser, TKey>, IUserLockoutStore<TUser, TKey>, IUserStore<TUser, TKey>, IDisposable
        where TUser : IdentityUser<TKey, TUserLogin, TUserClaim>
        where TKey : IEquatable<TKey>
        where TUserLogin : IdentityUserLogin, new()
        where TUserClaim : IdentityUserClaim, new()
    {
        bool Disposed;

        public IAsyncDocumentSession Session { get; set; }

        public UserStore(IAsyncDocumentSession session)
        {
            Session = session;
        }

        public IQueryable<TUser> Users
        {
            get
            {
                return Session.Query<TUser>();
            }
        }

        public virtual async Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            await Session.StoreAsync(user);
        }

        public virtual async Task UpdateAsync(TUser user)
        {
            await Task.FromResult(true);
        }

        public virtual async Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            Session.Delete(user);
            await Task.FromResult(true);
        }

        public virtual async Task<TUser> FindByIdAsync(TKey userId)
        {
            ThrowIfDisposed();
            return await Session.LoadAsync<TUser>(userId.ToString());
        }

        public virtual async Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return await Session.Query<TUser>().SingleOrDefaultAsync(u => u.UserName.Equals(userName, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual async Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            if (!user.Logins.Any(l => l.LoginProvider.Equals(login.LoginProvider, StringComparison.InvariantCultureIgnoreCase) && l.ProviderKey.Equals(login.ProviderKey, StringComparison.InvariantCultureIgnoreCase)))
            {
                var UserLoginInstance = Activator.CreateInstance<TUserLogin>();
                UserLoginInstance.ProviderKey = login.ProviderKey;
                UserLoginInstance.LoginProvider = login.LoginProvider;
                user.Logins.Add(UserLoginInstance);
            }
            await Task.FromResult(true);
        }

        public virtual async Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (login == null)
                throw new ArgumentNullException("login");

            var UserLogin = user.Logins.SingleOrDefault(l => l.LoginProvider.Equals(login.LoginProvider, StringComparison.InvariantCultureIgnoreCase) && l.ProviderKey.Equals(login.ProviderKey, StringComparison.InvariantCultureIgnoreCase));

            if (UserLogin != null)
                user.Logins.Remove(UserLogin);

            await Task.FromResult(true);
        }

        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult<IList<UserLoginInfo>>(Enumerable.ToList(Enumerable.Select(user.Logins, l => new UserLoginInfo(l.LoginProvider, l.ProviderKey))));
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();

            if (login == null)
                throw new ArgumentNullException("login");

            return await Session.Query<TUser>().SingleOrDefaultAsync(u => u.Logins.Any(l => l.LoginProvider.Equals(login.ProviderKey, StringComparison.InvariantCultureIgnoreCase) && l.ProviderKey.Equals(login.ProviderKey, StringComparison.InvariantCultureIgnoreCase)));
        }

        public virtual async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult<IList<Claim>>(Enumerable.ToList(Enumerable.Select(user.Claims, c => new Claim(c.ClaimType, c.ClaimValue))));
        }

        public virtual async Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (claim == null)
                throw new ArgumentNullException("claim");

            if (!user.Claims.Any(c => c.ClaimType.Equals(claim.Type, StringComparison.InvariantCultureIgnoreCase) && c.ClaimValue.Equals(claim.Value, StringComparison.InvariantCultureIgnoreCase)))
            {
                var UserClaimInstance = Activator.CreateInstance<TUserClaim>();
                UserClaimInstance.ClaimType = claim.Type;
                UserClaimInstance.ClaimValue = claim.Value;
                user.Claims.Add(UserClaimInstance);
            }
            await Task.FromResult(true);
        }

        public virtual async Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (claim == null)
                throw new ArgumentNullException("claim");

            var UserClaim = user.Claims.SingleOrDefault(c => c.ClaimType.Equals(claim.Type, StringComparison.InvariantCultureIgnoreCase) && c.ClaimValue.Equals(claim.Value, StringComparison.InvariantCultureIgnoreCase));

            if (UserClaim != null)
                user.Claims.Remove(UserClaim);

            await Task.FromResult(true);
        }

        public virtual async Task AddToRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleName");

            if (!user.Roles.Any(r => r.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)))
                user.Roles.Add(roleName);

            await Task.FromResult(true);
        }

        public virtual async Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleName");

            user.Roles.Remove(roleName);
            await Task.FromResult(true);
        }

        public virtual async Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult<IList<string>>(Enumerable.ToList(user.Roles));
        }

        public virtual async Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrWhiteSpace(roleName))
                throw new ArgumentException("ValueCannotBeNullOrEmpty", "roleName");

            return await Task.FromResult(user.Roles.Any(r => r.Equals(roleName, StringComparison.InvariantCultureIgnoreCase)));
        }

        public virtual async Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = passwordHash;
            await Task.FromResult(true);

        }

        public virtual async Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.PasswordHash);
        }

        public virtual async Task<bool> HasPasswordAsync(TUser user)
        {
            return await Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
        }

        public virtual async Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            await Task.FromResult(true);
        }

        public virtual async Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.SecurityStamp);
        }

        public virtual async Task SetEmailAsync(TUser user, string email)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.Email = email;
            await Task.FromResult(true);
        }

        public virtual async Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.Email);
        }

        public virtual async Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.EmailConfirmed);
        }

        public virtual async Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;
            await Task.FromResult(true);
        }

        public virtual async Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return await Session.Query<TUser>().SingleOrDefaultAsync(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }

        public virtual async Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.LockoutEndDateUtc.HasValue ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc)) : new DateTimeOffset());
        }

        public virtual async Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? new DateTime?() : new DateTime?(lockoutEnd.UtcDateTime);
            await Task.FromResult(true);
        }

        public virtual async Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            ++user.AccessFailedCount;
            return await Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task ResetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.AccessFailedCount = 0;
            await Task.FromResult(true);
        }

        public virtual async Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.AccessFailedCount);
        }

        public virtual async Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.LockoutEnabled);
        }

        public virtual async Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.LockoutEnabled = enabled;
            await Task.FromResult(true);
        }

        public virtual async Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.TwoFactorEnabled = enabled;
            await Task.FromResult(true);
        }

        public virtual async Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.TwoFactorEnabled);
        }

        public virtual async Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumber = phoneNumber;
            await Task.FromResult(true);
        }

        public virtual async Task<string> GetPhoneNumberAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.PhoneNumber);
        }

        public virtual async Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            return await Task.FromResult(user.PhoneNumberConfirmed);
        }

        public virtual async Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();

            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;
            await Task.FromResult(true);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && Session != null)
            {
                Session.SaveChangesAsync();
                Session.Dispose();
                Session = null;
            }

            Disposed = true;
        }

        void ThrowIfDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException(GetType().Name);
        }
    }

    [SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "False positive.  IDisposable is inherited via UserStore<TUser, string, IdentityUserLogin, IdentityUserClaim>.")]
    public class UserStore<TUser> : UserStore<TUser, string, IdentityUserLogin, IdentityUserClaim>, IDisposable, IUserStore<TUser>, IUserStore<TUser, string> where TUser : IdentityUser
    {
        public UserStore(IAsyncDocumentSession session)
            : base(session)
        {
        }
    }
}
