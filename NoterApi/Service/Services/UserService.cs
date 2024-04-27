
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Identity;
using Microsoft.Extensions.Configuration;
using Repository.Infrastructure;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNet.Identity;
using Service.Models;

namespace Service.Services
{
    public interface IUserService
    {
        Task<Microsoft.AspNetCore.Identity.IdentityResult> CreateAsync(User user, string password);
        Task<string> ValidatePassword(string password);
        Task<Microsoft.AspNetCore.Identity.IdentityResult> AddToRoleAsync(User user, string roleName);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByNameAsync(string normalizedUserName);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<User> FindByIdAsync(string userId);
        Task<IList<string>> GetRolesAsync(User user);
        Task<string> GetConfirmationCode(string email, string userId);
        Task<bool> CheckConfirmationCode(string userId, string confirmationCode);
        Task<Microsoft.AspNetCore.Identity.IdentityResult> ResetPasswordWithoutOldPasswordAsync(string userid, string newPassword);
        Task ValidateUser(string userId);
        Task<UserInfo> GetUser();
        Task<long> DeleteUser(long id);
        Task<GoogleJsonWebSignature.Payload> VerifyToken(string token);
    }


    public class UserService : IUserService
    {
        private readonly UserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Microsoft.AspNetCore.Identity.IUserStore<User> _userStore;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _httpContextAccessor;


        public UserService(UserManager userManager, SignInManager<User> signInManager,
             IConfiguration configuration, IUnitOfWork unitOfWork, Microsoft.AspNetCore.Identity.IUserStore<User> userStore
, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _userStore = userStore;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyToken(string token)
        {

            try
            {
                GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(token);
                return payload;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task<UserInfo> GetUser()
        {

            try
            {
                UserInfo userInfo = null ;
               var identityUser = _httpContextAccessor.HttpContext?.User;

                var userId = identityUser?.Identity.GetUserId();
                var user = await FindByIdAsync(userId);
                if (user != null)
                {
                     userInfo=new UserInfo() { Email = user.Email, FullName = user.Name + " " + user.Surname };
                }
                return userInfo;
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> CreateAsync(User user, string password)
        {
            try
            {
                var result = await _userManager.CreateAsync(user, password);
                return result;

            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<User> FindByEmailAsync(string email)
        {
            var result = await _userManager.FindByEmailAsync(email);
            return result;
        }

        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> AddToRoleAsync(User user, string roleName)
        {
            var result = await _userManager.AddToRoleAsync(user, roleName);
            return result;
        }

        public async Task<string> ValidatePassword(string password)
        {
            List<string> passwordErrors = new List<string>();

            var validators = _userManager.PasswordValidators;

            foreach (var validator in validators)
            {
                var validation = await validator.ValidateAsync(_userManager, null, password);

                if (!validation.Succeeded)
                {
                    foreach (var error in validation.Errors)
                    {
                        passwordErrors.Add(error.Description);
                    }
                }
            }

            var result = passwordErrors.Count > 0 ? passwordErrors.Aggregate((i, j) => i + "\n" + j) : null;

            return result;
        }
        public async Task<User> FindByNameAsync(string normalizedUserName)
        {
            var result = await _userManager.FindByNameAsync(normalizedUserName);
            return result;
        }
        public async Task<User> FindByIdAsync(string userId)
        {
            var result = await _userManager.FindByIdAsync(userId);
            return result;
        }
        public async Task<IList<string>> GetRolesAsync(User user)
        {
            var result = await _userManager.GetRolesAsync(user);
            return result;
        }
        public async Task<string> GetConfirmationCode(string email, string userId)
        {
            var res = await _userManager.GetConfirmationCode(email, userId);
            return res;
        }
        public async Task<bool> CheckConfirmationCode(string userId, string confirmationCode)
        {
            var res = await _userManager.CheckConfirmationCode(userId, confirmationCode);
            return res;
        }

        public async Task<Microsoft.AspNetCore.Identity.IdentityResult> ResetPasswordWithoutOldPasswordAsync(string userId, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            return result;
        }
        public async Task ValidateUser(string userId)
        {
            await _userManager.ValidateUser(userId);
        }




        public async Task<long> DeleteUser(long id)
        {
            try
            {
                var result = await _userManager.DeleteUser(id);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

}
