
using Dapper;
using Core.Models;
using Repository.Infrastructure;

namespace Repository.CQRS.Queries
{
    public interface ITokenQuery
    {
        UserToken FindByKeys(string loginProvider, string refreshToken);
    }

    public class TokenQuery : ITokenQuery
    {
        private const string ByKeysSql = @"SELECT * FROM UserTokens WHERE LoginProvider = @loginProvider AND [Value] = @refreshToken";
        private readonly IUnitOfWork _unitOfWork;

        public TokenQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public UserToken FindByKeys(string loginProvider, string refreshToken)
        {
            var parameters = new
            {
                loginProvider,
                refreshToken
            };

            var result = _unitOfWork.GetConnection().QuerySingle<UserToken>(ByKeysSql, parameters, _unitOfWork.GetTransaction());
            return result;
        }
    }
}
