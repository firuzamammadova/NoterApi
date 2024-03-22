using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Repository.Identity
{
    public class UserManager : UserManager<User>
    {

        private readonly IAppUserStore<User> _userStore;

        public UserManager(IAppUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            _userStore = store;
        }

        public async Task<string> GetConfirmationCode(string email, string userId)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                var res = await _userStore.GetConfirmationCode(email, userId, cancellationToken.Token);
                return res;
            }
        }
        public async Task<bool> CheckConfirmationCode(string userId, string confirmationCode)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                var res = await _userStore.CheckConfirmationCode(userId, confirmationCode, cancellationToken.Token);
                return res;
            }
        }
        public async Task ValidateUser(string userId)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                await _userStore.ValidateUser(userId, cancellationToken.Token);
            }
        }




        public async Task<long> DeleteUser(long id)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                var result = await _userStore.DeleteUser(id, cancellationToken.Token);
                return result;
            }
        }
    }
}
