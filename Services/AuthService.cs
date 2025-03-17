using System.Threading.Tasks;
using Microsoft.Maui.Storage;

namespace Elkollen.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string token);
        Task LogoutAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<string> GetTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private const string TokenKey = "auth_token";
        private const string IsAuthenticatedKey = "is_authenticated";

        public async Task<bool> LoginAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            try
            {
                await SecureStorage.Default.SetAsync(TokenKey, token);
                Preferences.Default.Set(IsAuthenticatedKey, true);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            Preferences.Default.Set(IsAuthenticatedKey, false);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            return Preferences.Default.Get(IsAuthenticatedKey, false);
        }

        public async Task<string> GetTokenAsync()
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }
    }
}