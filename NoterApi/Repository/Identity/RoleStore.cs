using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Models;
using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Repository.Identity
{
    public interface IAppRoleStore<TRole> : IRoleStore<TRole> where TRole : class
    {
        Task<Role> FindUniqueByNameAsync(string normalizedUserName, string roleId, CancellationToken cancellationToken);
    }

    public class RoleStore : IAppRoleStore<Role>, IQueryableRoleStore<Role>
    {
        private readonly string _connectionString;

        public IQueryable<Role> Roles
        {
            get
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var result = connection.Query<Role>($@"SELECT R.ID,R.NAME,R.NORMALIZEDNAME,R.GROUPID,RG.NAME 'GROUPNAME',R.ROWNUM FROM [ROLES] R 
LEFT JOIN ROLEGROUPS RG ON R.GROUPID=RG.ID ORDER BY R.RowNum", null).AsQueryable();
                    return result;
                }
            }
        }

        public RoleStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                role.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO [Roles] ([Name], [NormalizedName], [GroupId])
                    VALUES (@{nameof(Role.Name)}, @{nameof(Role.NormalizedName)}, @{nameof(Role.GroupId)});
                    SELECT CAST(SCOPE_IDENTITY() as int)", role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> UpdateAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"UPDATE [Roles] SET
                    [Name] = @{nameof(Role.Name)},
                    [NormalizedName] = @{nameof(Role.NormalizedName)},
                    [GroupId] = @{nameof(Role.GroupId)}
                    WHERE [Id] = @{nameof(Role.Id)}", role);
            }

            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteAsync(Role role, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($"DELETE FROM [Roles] WHERE [Id] = @{nameof(Role.Id)}", role);
            }

            return IdentityResult.Success;
        }

        public Task<string> GetRoleIdAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Id.ToString());
        }

        public Task<string> GetRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.Name);
        }

        public Task SetRoleNameAsync(Role role, string roleName, CancellationToken cancellationToken)
        {
            role.Name = roleName;
            return Task.FromResult(0);
        }

        public Task<string> GetNormalizedRoleNameAsync(Role role, CancellationToken cancellationToken)
        {
            return Task.FromResult(role.NormalizedName);
        }

        public Task SetNormalizedRoleNameAsync(Role role, string normalizedName, CancellationToken cancellationToken)
        {
            role.NormalizedName = normalizedName;
            return Task.FromResult(0);
        }

        public async Task<Role> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<Role>($@"SELECT R.ID,R.Name,R.NormalizedName,R.GroupId,RG.NAME 'GROUPNAME' FROM [Roles] R
                        LEFT JOIN RoleGroups RG ON R.GroupId=RG.Id
                    WHERE R.[Id] = @{nameof(roleId)}", new { roleId });
            }
        }

        public async Task<Role> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<Role>($@"SELECT * FROM [Roles]
                    WHERE [NormalizedName] = @{nameof(normalizedRoleName)}", new { normalizedRoleName });
            }
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }


        public async Task<Role> FindUniqueByNameAsync(string normalizedUserName, string roleId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return await connection.QuerySingleOrDefaultAsync<Role>($@"SELECT * FROM [Roles] 
                    WHERE [NormalizedName] = @{nameof(normalizedUserName)} and [id]<>@{nameof(roleId)}", new { normalizedUserName, roleId });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
