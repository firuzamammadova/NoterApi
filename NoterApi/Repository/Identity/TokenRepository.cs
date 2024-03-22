using Core.Models;
using Repository.CQRS.Commands;
using Repository.CQRS.Queries;
using System;


namespace Repository.Identity
{
    public interface ITokenRepository
    {
        void Add(UserToken appUserToken);
        UserToken FindByKeys(string loginProvider, string refreshToken);
        void Remove(UserToken appUserToken);
        void RemoveByRefreshToken(string refreshToken);
    }

    public class TokenRepository : ITokenRepository
    {
        private readonly ITokenCommand _tokenCommand;
        private readonly ITokenQuery _tokenQuery;

        public TokenRepository(ITokenCommand tokenCommand, ITokenQuery tokenQuery)
        {
            _tokenCommand = tokenCommand;
            _tokenQuery = tokenQuery;
        }

        public void Add(UserToken appUserToken)
        {
            _tokenCommand.Execute(appUserToken.UserId, appUserToken.LoginProvider, appUserToken.Name, appUserToken.Value, DateTime.Now, appUserToken.Type);
        }

        public UserToken FindByKeys(string loginProvider, string refreshToken)
        {
            var result = _tokenQuery.FindByKeys(loginProvider, refreshToken);
            return result;
        }

        public void Remove(UserToken appUserToken)
        {
            _tokenCommand.Remove(appUserToken.UserId, appUserToken.LoginProvider, appUserToken.Name);
        }

        public void RemoveByRefreshToken(string refreshToken)
        {
            _tokenCommand.RemoveByRefreshToken(refreshToken);
        }
    }

}
