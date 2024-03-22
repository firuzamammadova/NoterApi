
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Identity;
using Microsoft.Extensions.Configuration;
using Repository.Infrastructure;

namespace Service.Services
{
    public interface IUserService
    {
        Task<IdentityResult> CreateAsync(User user, string password);
        Task<string> ValidatePassword(string password);
        Task<IdentityResult> AddToRoleAsync(User user, string roleName);
        Task<User> FindByEmailAsync(string email);
        Task<User> FindByNameAsync(string normalizedUserName);
        Task<bool> CheckPasswordAsync(User user, string password);
        Task<User> FindByIdAsync(string userId);
        Task<IList<string>> GetRolesAsync(User user);
        Task<string> GetConfirmationCode(string email, string userId);
        Task<bool> CheckConfirmationCode(string userId, string confirmationCode);
        Task<IdentityResult> ResetPasswordWithoutOldPasswordAsync(string userid, string newPassword);
        Task ValidateUser(string userId);
        Task<long> DeleteUser(long id);
    }


    public class UserService : IUserService
    {
        private readonly UserManager _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;


        public UserService(UserManager userManager, SignInManager<User> signInManager,
             IConfiguration configuration, IUnitOfWork unitOfWork, IUserStore<User> userStore
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
            _userStore = userStore;
            _configuration = configuration;
        }

        public async Task<bool> CheckPasswordAsync(User user, string password)
        {
            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResult> CreateAsync(User user, string password)
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

        public async Task<IdentityResult> AddToRoleAsync(User user, string roleName)
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

        public async Task<IdentityResult> ResetPasswordWithoutOldPasswordAsync(string userId, string newPassword)
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
