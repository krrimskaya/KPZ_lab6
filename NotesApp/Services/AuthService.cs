using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace NotesApp.Services
{
    public class AuthService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthService(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IdentityResult> RegisterUserAsync(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email };
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}