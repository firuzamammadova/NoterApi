using System;
using System.Collections.Generic;
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
    public interface IAppUserStore<TUser> : IUserStore<TUser> where TUser : class
    {
        Task<User> FindUniqueByNameAsync(string normalizedUserName, string userId, CancellationToken cancellationToken);
        Task<User> FindUniqueByEmailAsync(string email, string userId, CancellationToken cancellationToken);
       // Task<IList<ApplicationRoleUI>> GetRolesByIdAsync(string userId, CancellationToken cancellationToken);
        Task<User> Block(User user, CancellationToken cancellationToken);
        Task<string> GetConfirmationCode(string email, string userId, CancellationToken cancellationToken);
        Task<bool> CheckConfirmationCode(string userId, string confirmationCode, CancellationToken cancellationToken);
        Task<DateTime> GetRemainingTime(string userId, CancellationToken cancellationToken);
        Task ValidateUser(string userId, CancellationToken cancellationToken);
        Task<long> DeleteUser(long id, CancellationToken cancellationToken);
    }

    public class UserStore : IAppUserStore<User>, IUserEmailStore<User>, IUserPhoneNumberStore<User>,
       IUserTwoFactorStore<User>, IUserPasswordStore<User>, IUserAuthenticationTokenStore<UserToken>
    {
        private readonly string _connectionString;

        public UserStore(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    user.Id = await connection.QuerySingleAsync<Guid>($@"
                                                                       DECLARE @MyTableVar table([ID] [int]);
                                                                       INSERT INTO [dbo].[Users]
                                                                      ( [Email]
                                                                       , [Name] , [Surname],[BirthDate],[GenderIdentity]
                                                                         ,[PasswordRecovery],[DeleteStatus],[Blocked]
                                                                      , [NormalizedEmail],[EmailConfirmed] ,[LockoutEnabled] 
                                                                      , [PasswordHash] , [PhoneNumber] , [PhoneNumberConfirmed]
                                                                      , [TwoFactorEnabled]
                                                                       ,[UserName],[NormalizedUserName]
                                                                      ,[SecurityStamp],[Active])
                                                                       OUTPUT INSERTED.[Id] 
                                                                       VALUES
                                                                      (@{nameof(User.Email)}
                                                                        ,@{nameof(User.Name)}
                                                                        ,@{nameof(User.Surname)}
                                                                        ,@{nameof(User.BirthDate)}
                                                                        ,@{nameof(User.GenderIdentity)}
                                                                        ,@{nameof(User.PasswordRecovery)}
                                                                        ,0
                                                                        ,0

                                                                            
                                                                        ,@{nameof(User.Email)}
                                                                      ,1 ,1 
                                                                      ,@{nameof(User.PasswordHash)}
                                                                      ,@{nameof(User.PhoneNumber)} ,0 ,0
                                                                      ,@{nameof(User.UserName)}
                                                                      ,@{nameof(User.UserName)}
                                                                        
                                                                      ,@{nameof(User.SecurityStamp)},0)
                                                                       ", user);
                }

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Something went wrong" });
            }
        }

        public async Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                await connection.ExecuteAsync($@"DELETE FROM Users
                                                 WHERE Id = @{nameof(User.Id)}", user);
                                                            }

            return IdentityResult.Success;
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                var res = await connection.QuerySingleOrDefaultAsync<User>($@"SELECT [Users].* FROM [Users]
                    WHERE [Users].[Id] = @{nameof(userId)} and [Users].[DeleteStatus]=0", new { userId });
                return res;
            }
        }
        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                try
                {
                    return await connection.QuerySingleOrDefaultAsync<User>($@"SELECT [Users].* FROM [Users]
                    WHERE [Users].[NormalizedUserName] = @{nameof(normalizedUserName)} and [Users].[DeleteStatus]=0", new { normalizedUserName });
                }
                catch (Exception ex)
                {

                    throw ex;
                }

            }
        }

        public async Task<User> FindUniqueByNameAsync(string normalizedUserName, string userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return await connection.QuerySingleOrDefaultAsync<User>($@"SELECT [Users].*,[Group].Name [Group] FROM [Users]
                    LEFT JOIN [Group] ON [Group].Id = [Users].GroupId 
                    WHERE [Users].[NormalizedUserName] = @{nameof(normalizedUserName)} and [Users].[Id]<>@{nameof(userId)}", new { normalizedUserName, userId });
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<User> FindUniqueByEmailAsync(string email, string userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);
                    return await connection.QuerySingleOrDefaultAsync<User>($@"SELECT [Users].*,[Group].Name [Group] FROM [Users]
                    LEFT JOIN [Group] ON [Group].Id = [Users].DepartmentId 
                    WHERE [Users].[Email] = @{nameof(email)} and [Users].[Id]<>@{nameof(userId)}", new { email, userId });
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.FromResult(0);
        }

        public async Task<User> Block(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            User res = new User();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                user.Blocked = user.Blocked == true ? false : true;
                res = await connection.QueryFirstOrDefaultAsync<User>($@"UPDATE [Users] SET
                    [Blocked]=@{nameof(User.Blocked)}
                    WHERE [Id] = @{nameof(User.Id)}
                    SELECT * FROM [Users] WHERE [Id] = @{nameof(User.Id)}", user);
            }

            return res;
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                //await connection.OpenAsync(cancellationToken);
                if (user.PasswordHash == null)
                {
                    //await connection.OpenAsync(cancellationToken);
                    //await connection.ExecuteAsync($@"UPDATE [AppUsers] SET
                    //[UserName] = @{nameof(User.UserName)},
                    //[NormalizedUserName] = @{nameof(User.NormalizedUserName)},
                    //[Email] = @{nameof(User.Email)},
                    //[NormalizedEmail] = @{nameof(User.NormalizedEmail)},
                    //[EmailConfirmed] = @{nameof(User.EmailConfirmed)},
                    //[PhoneNumber] = @{nameof(User.PhoneNumber)},
                    //[PasswordHash] = @{nameof(User.PasswordHash)},
                    //[PhoneNumberConfirmed] = @{nameof(User.PhoneNumberConfirmed)},
                    //[TwoFactorEnabled] = @{nameof(User.TwoFactorEnabled)},
                    //[PasswordRecovery] = @{nameof(User.PasswordRecovery)},
                    //[DisplayName] = @{nameof(User.DisplayName)},
                    //[UserSurName] = @{nameof(User.UserSurName)},
                    //[SocialNetwork] = @{nameof(User.SocialNetwork)},
                    //[CreatedDate] = @{nameof(User.CreatedDate)},
                    //[CreatedBy] = @{nameof(User.CreatedBy)},
                    //[LastModifiedDate] = @{nameof(User.LastModifiedDate)}
                    //WHERE [Id] = @{nameof(User.Id)}", user);
                }
                else
                {
                    await connection.ExecuteAsync($@"UPDATE [Users] SET
                    [PasswordHash] = @{nameof(User.PasswordHash)},
                    [Active]=@{nameof(User.Active)}
                    WHERE [Id] = @{nameof(User.Id)}", user);
                }
            }

            return IdentityResult.Success;
        }

        public Task SetEmailAsync(User user, string email, CancellationToken cancellationToken)
        {
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public async Task<User> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<User>(
                    $@"SELECT [Users].* FROM [Users]
                    WHERE [Users].[Email] = @{nameof(normalizedEmail)} and [Users].[DeleteStatus]=0", new { normalizedEmail });
            }
        }

        public Task<string> GetNormalizedEmailAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetNormalizedEmailAsync(User user, string normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail;
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(User user, string phoneNumber, CancellationToken cancellationToken)
        {
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(User user, bool confirmed, CancellationToken cancellationToken)
        {
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(User user, bool enabled, CancellationToken cancellationToken)
        {
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }






        public Task SetTokenAsync(UserToken user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveTokenAsync(UserToken user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTokenAsync(UserToken user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                var result = await connection.QuerySingleAsync<UserToken>("SELECT * FROM UserTokens WHERE userId = @userId && loginProvider = @loginProvider && Name = @name",
                    new { userId = user.Id, loginProvider = loginProvider, name = name });

                return result.Value;
            }
        }

        public async Task<bool> CheckEmailVerification(string email, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    var query = $@"SELECT COUNT(*) FROM Users
                                WHERE Email = '{email}' AND [DeleteStatus] = 0";

                    var res = await connection.QueryFirstAsync<int>(query);
                    if (res == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<string> GetConfirmationCode(string email, string userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    var emailVerification = await CheckEmailVerification(email, cancellationToken);

                    if (emailVerification == true)
                    {

                        var check = await CheckPreventReplayingConfirmationCode(userId, "000000", cancellationToken);

                        if (check == false)
                        {

                            var tmp = DateTime.Now.AddMinutes(3);
                            var expirationDate = tmp.Year + "-" + tmp.Month + "-" + tmp.Day + " " + tmp.Hour + ":" + tmp.Minute + ":" + tmp.Second;

                            var confirmationSymbols = new Random().Next(100000, 999999);

                            await connection.OpenAsync(cancellationToken);

                            var query = @$" INSERT INTO RESETPASSWORDS(USERID,CONFIRMATIONSYMBOLS,EXPIREDDATE)
                                OUTPUT Inserted.CONFIRMATIONSYMBOLS
                                VALUES('{userId}','{confirmationSymbols}','{expirationDate}')";

                            var res = await connection.QueryFirstAsync<string>(query);
                            return res;
                        }
                        else
                        {

                            var currentDate = DateTime.Now;
                            var expriedDate = await GetRemainingTime(userId, cancellationToken);

                            var remainingTime = expriedDate.Subtract(currentDate);
                            var expirationDate = (int)remainingTime.TotalSeconds / 60 + ":" + (int)remainingTime.TotalSeconds % 60;

                            throw new Exception("Please wait!" + expirationDate);
                        }
                    }
                    else
                    {
                        throw new Exception($"Email is not verified : {email}");
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<bool> CheckConfirmationCode(string userId, string confirmationCode, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    var tmp = DateTime.Now;
                    var now = tmp.Year + "-" + tmp.Month + "-" + tmp.Day + " " + tmp.Hour + ":" + tmp.Minute + ":" + tmp.Second;

                    await connection.OpenAsync(cancellationToken);

                    var query = @$"SELECT COUNT(*) FROM RESETPASSWORDS 
                                   WHERE USERID LIKE '{userId}' AND CONFIRMATIONSYMBOLS = '{confirmationCode}' AND EXPIREDDATE > '{now}' AND DELETESTATUS = 0";

                    var res = await connection.QueryFirstAsync<int>(query);
                    if (res == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public async Task ValidateUser(string userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync(cancellationToken);

                    var getQuery = $@"SELECT TOP 1 Id FROM ResetPasswords
                                   WHERE UserId = '{userId}' AND Verified = 0 AND DeleteStatus = 0
                                   ORDER BY ExpiredDate DESC";

                    var res = await connection.QueryFirstAsync<long>(getQuery);

                    var setQuery = @$"UPDATE ResetPasswords
                                      Set Verified = 1
                                      Where Id = '{res}'";

                    await connection.ExecuteAsync(setQuery);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<bool> CheckPreventReplayingConfirmationCode(string userId, string confirmationCode, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    var tmp = DateTime.Now;
                    var now = tmp.Year + "-" + tmp.Month + "-" + tmp.Day + " " + tmp.Hour + ":" + tmp.Minute + ":" + tmp.Second;

                    await connection.OpenAsync(cancellationToken);

                    var query = @$"SELECT COUNT(*) FROM RESETPASSWORDS 
                                   WHERE USERID LIKE '{userId}' AND EXPIREDDATE > '{now}' AND DELETESTATUS = 0";

                    var res = await connection.QueryFirstAsync<int>(query);
                    if (res == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public Task<string> GetUserIdAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(UserToken user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(UserToken user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(UserToken user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserToken> IUserStore<UserToken>.FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<UserToken> IUserStore<UserToken>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }


        public void Dispose()
        {
            // Nothing to dispose.
        }

        public async Task<DateTime> GetRemainingTime(string userId, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using (var connection = new SqlConnection(_connectionString))
                {
                    var tmp = DateTime.Now;
                    var now = tmp.Year + "-" + tmp.Month + "-" + tmp.Day + " " + tmp.Hour + ":" + tmp.Minute + ":" + tmp.Second;

                    await connection.OpenAsync(cancellationToken);

                    var query = @$"SELECT TOP 1 ExpiredDate FROM ResetPasswords
                                   WHERE UserId = '{userId}' AND DeleteStatus = 0 AND Verified = 0
                                   ORDER BY ExpiredDate DESC
                                   ";

                    var res = await connection.QueryFirstAsync<DateTime>(query);

                    return res;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public async Task<long> DeleteUser(long id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    var query = @"Update Users set DeleteStatus=1 WHERE Id=@id";
                    await connection
                         .QueryAsync(query, new { id });
                    return id;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }
    }

}
