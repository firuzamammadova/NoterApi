using Dapper;
using System;
using Repository.Infrastructure;

namespace Repository.CQRS.Commands
{
    public interface ITokenCommand
    {
        void Execute(string userId, string loginProvider, string name, string value, DateTime addedDate, int type);
        void Remove(string userId, string loginProvider, string name);
        void RemoveByRefreshToken(string refreshToken);
    }

    public class TokenCommand : ITokenCommand
    {
        private const string CreateSql = @"
INSERT INTO dbo.UserTokens
VALUES
(   @userId,
    @loginProvider,
    @name,
    @value,
    @addedDate, 
    @type 
)";

        private const string RemoveSql = @"DELETE FROM dbo.UserTokens WHERE UserId=@userId AND loginProvider = @loginProvider AND [Name]=@name";

        private const string RemoveByValue = @"DELETE FROM dbo.UserTokens WHERE Value = @refreshToken";

        private readonly IUnitOfWork _unitOfWork;

        public TokenCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Execute(string userId, string loginProvider, string name, string value, DateTime addedDate,
            int type)
        {
            try
            {



                SqlMapper.AddTypeMap(typeof(DateTime), System.Data.DbType.DateTime2);
                var parameters = new
                {
                    userId,
                    loginProvider,
                    name,
                    value,
                    addedDate,
                    type
                };

                _unitOfWork.GetConnection().Execute(CreateSql, parameters, _unitOfWork.GetTransaction());
            }
            catch (Exception ex)
            {

            }
        }

        public void Remove(string userId, string loginProvider, string name)
        {
            var parameters = new
            {
                userId,
                loginProvider,
                name
            };
            _unitOfWork.GetConnection().Execute(RemoveSql, parameters, _unitOfWork.GetTransaction());
        }

        public void RemoveByRefreshToken(string refreshToken)
        {
            var parameters = new
            {
                refreshToken
            };
            _unitOfWork.GetConnection().Execute(RemoveByValue, parameters, _unitOfWork.GetTransaction());
        }
    }
}

