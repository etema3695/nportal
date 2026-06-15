using Microsoft.AspNetCore.Identity;
using NewsPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NewsPortal.Identity
{
    /// <summary>
    /// EF6-backed IUserStore for ASP.NET Core Identity.
    /// Bridges ASP.NET Core Identity (service layer) with the existing EF6 ApplicationDbContext.
    /// </summary>
    public class Ef6UserStore :
        IUserStore<ApplicationUser>,
        IUserPasswordStore<ApplicationUser>,
        IUserEmailStore<ApplicationUser>,
        IUserRoleStore<ApplicationUser>,
        IUserSecurityStampStore<ApplicationUser>,
        IUserLockoutStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>
    {
        private readonly ApplicationDbContext _db;

        public Ef6UserStore(ApplicationDbContext db) => _db = db;

        public void Dispose() { }

        // --- IUserStore ---
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken ct)
        {
            _db.Set<ApplicationUser>().Add(user);
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken ct)
        {
            var entry = _db.Entry(user);
            if (entry.State == EntityState.Detached)
                _db.Set<ApplicationUser>().Attach(user);
            entry.State = EntityState.Modified;
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken ct)
        {
            _db.Set<ApplicationUser>().Remove(user);
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken ct)
            => Task.FromResult(_db.Set<ApplicationUser>().Find(userId));

        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken ct)
            => Task.FromResult(_db.Set<ApplicationUser>()
                .FirstOrDefault(u => u.UserName.ToUpper() == normalizedUserName.ToUpper()
                                  || (u.NormalizedUserName != null && u.NormalizedUserName == normalizedUserName)));

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.Id);

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.UserName);

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken ct)
        { user.UserName = userName; return Task.CompletedTask; }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.NormalizedUserName);

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken ct)
        { user.NormalizedUserName = normalizedName; return Task.CompletedTask; }

        // --- IUserPasswordStore ---
        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken ct)
        { user.PasswordHash = passwordHash; return Task.CompletedTask; }

        public Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.PasswordHash);

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.PasswordHash != null);

        // --- IUserEmailStore ---
        public Task SetEmailAsync(ApplicationUser user, string email, CancellationToken ct)
        { user.Email = email; return Task.CompletedTask; }

        public Task<string> GetEmailAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.Email);

        public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.EmailConfirmed);

        public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken ct)
        { user.EmailConfirmed = confirmed; return Task.CompletedTask; }

        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken ct)
            => Task.FromResult(_db.Set<ApplicationUser>()
                .FirstOrDefault(u => u.Email.ToUpper() == normalizedEmail.ToUpper()
                                  || (u.NormalizedEmail != null && u.NormalizedEmail == normalizedEmail)));

        public Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.NormalizedEmail);

        public Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken ct)
        { user.NormalizedEmail = normalizedEmail; return Task.CompletedTask; }

        // --- IUserRoleStore ---
        public Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
        {
            var role = _db.Set<IdentityRole>().FirstOrDefault(r => r.NormalizedName == roleName.ToUpperInvariant());
            if (role != null)
            {
                var existing = _db.Set<ApplicationUserRole>()
                    .FirstOrDefault(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
                if (existing == null)
                    _db.Set<ApplicationUserRole>().Add(new ApplicationUserRole { UserId = user.Id, RoleId = role.Id });
            }
            _db.SaveChanges();
            return Task.CompletedTask;
        }

        public Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
        {
            var role = _db.Set<IdentityRole>().FirstOrDefault(r => r.NormalizedName == roleName.ToUpperInvariant());
            if (role != null)
            {
                var ur = _db.Set<ApplicationUserRole>()
                    .FirstOrDefault(x => x.UserId == user.Id && x.RoleId == role.Id);
                if (ur != null) _db.Set<ApplicationUserRole>().Remove(ur);
            }
            _db.SaveChanges();
            return Task.CompletedTask;
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken ct)
        {
            var roleIds = _db.Set<ApplicationUserRole>()
                .Where(ur => ur.UserId == user.Id).Select(ur => ur.RoleId).ToList();
            IList<string> roles = _db.Set<IdentityRole>()
                .Where(r => roleIds.Contains(r.Id)).Select(r => r.Name).ToList();
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken ct)
        {
            var role = _db.Set<IdentityRole>().FirstOrDefault(r => r.NormalizedName == roleName.ToUpperInvariant());
            if (role == null) return Task.FromResult(false);
            var exists = _db.Set<ApplicationUserRole>()
                .Any(ur => ur.UserId == user.Id && ur.RoleId == role.Id);
            return Task.FromResult(exists);
        }

        public Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken ct)
        {
            var role = _db.Set<IdentityRole>().FirstOrDefault(r => r.NormalizedName == roleName.ToUpperInvariant());
            IList<ApplicationUser> users = new List<ApplicationUser>();
            if (role != null)
            {
                var userIds = _db.Set<ApplicationUserRole>()
                    .Where(ur => ur.RoleId == role.Id).Select(ur => ur.UserId).ToList();
                users = _db.Set<ApplicationUser>().Where(u => userIds.Contains(u.Id)).ToList();
            }
            return Task.FromResult(users);
        }

        // --- IUserSecurityStampStore ---
        public Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken ct)
        { user.SecurityStamp = stamp; return Task.CompletedTask; }

        public Task<string> GetSecurityStampAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.SecurityStamp);

        // --- IUserLockoutStore ---
        public Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.LockoutEnd);

        public Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken ct)
        { user.LockoutEnd = lockoutEnd; return Task.CompletedTask; }

        public Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken ct)
        { user.AccessFailedCount++; return Task.FromResult(user.AccessFailedCount); }

        public Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken ct)
        { user.AccessFailedCount = 0; return Task.CompletedTask; }

        public Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.AccessFailedCount);

        public Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.LockoutEnabled);

        public Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken ct)
        { user.LockoutEnabled = enabled; return Task.CompletedTask; }

        // --- IUserTwoFactorStore ---
        public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken ct)
        { user.TwoFactorEnabled = enabled; return Task.CompletedTask; }

        public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken ct)
            => Task.FromResult(user.TwoFactorEnabled);
    }

    /// <summary>
    /// EF6-backed IRoleStore for ASP.NET Core Identity.
    /// </summary>
    public class Ef6RoleStore : IRoleStore<IdentityRole>
    {
        private readonly ApplicationDbContext _db;
        public Ef6RoleStore(ApplicationDbContext db) => _db = db;
        public void Dispose() { }

        public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken ct)
        {
            _db.Set<IdentityRole>().Add(role);
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken ct)
        {
            var entry = _db.Entry(role);
            if (entry.State == EntityState.Detached) _db.Set<IdentityRole>().Attach(role);
            entry.State = EntityState.Modified;
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken ct)
        {
            _db.Set<IdentityRole>().Remove(role);
            _db.SaveChanges();
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken ct)
            => Task.FromResult(_db.Set<IdentityRole>().Find(roleId));

        public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken ct)
            => Task.FromResult(_db.Set<IdentityRole>()
                .FirstOrDefault(r => r.NormalizedName == normalizedRoleName));

        public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken ct)
            => Task.FromResult(role.Id);

        public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken ct)
            => Task.FromResult(role.Name);

        public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken ct)
        { role.Name = roleName; return Task.CompletedTask; }

        public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken ct)
            => Task.FromResult(role.NormalizedName);

        public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken ct)
        { role.NormalizedName = normalizedName; return Task.CompletedTask; }
    }
}
