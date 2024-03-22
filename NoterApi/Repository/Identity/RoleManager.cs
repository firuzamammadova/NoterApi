using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Repository.Identity
{
    public class RoleManager : RoleManager<Role>
    {
        private readonly IAppRoleStore<Role> _roleStore;

        public RoleManager(IAppRoleStore<Role> store, IEnumerable<IRoleValidator<Role>> roleValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, ILogger<RoleManager<Role>> logger) : base(store, roleValidators, keyNormalizer, errors, logger)
        {
            _roleStore = store;
        }

        public async Task<Role> FindUniqueByNameAsync(string normalizedUserName, string roleId)
        {
            using (var cancellationToken = new CancellationTokenSource())
            {
                var result = await _roleStore.FindUniqueByNameAsync(normalizedUserName, roleId, cancellationToken.Token);

                return result;
            }
        }
    }
}
