using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NotesApp.Services
{
    public class PasswordService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public PasswordService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> ResetPasswordAsync(string email, string code, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return IdentityResult.Failed();

            return await _userManager.ResetPasswordAsync(user, code, password);
        }

        public async Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
    }
}