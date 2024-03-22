using System;
using Core.Models;
using Repository.Identity;
using Repository.Infrastructure;

namespace Services.Service
{
    public interface ITokenService
    {
        void Add(UserToken appUserToken);
        UserToken FindByKeys(string loginProvider, string refreshToken);
        void Remove(UserToken appUserToken);
        void RemoveByRefreshToken(string refreshToken);
    }

    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(IUnitOfWork unitOfWork, ITokenRepository tokenRepository)
        {
            _unitOfWork = unitOfWork;
            _tokenRepository = tokenRepository;
        }

        public void Add(UserToken appUserToken)
        {
            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _tokenRepository.Add(appUserToken);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }
        }

        public UserToken FindByKeys(string loginProvider, string refreshToken)
        {
            using (_unitOfWork.BeginTransaction())
            {
                var result = _tokenRepository.FindByKeys(loginProvider, refreshToken);
                return result;
            }
        }

        public void Remove(UserToken appUserToken)
        {
            using (var tran = _unitOfWork.BeginTransaction())
            {
                try
                {
                    _tokenRepository.Remove(appUserToken);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }

            }
        }

        public void RemoveByRefreshToken(string refreshToken)
        {
            using (var tran = _unitOfWork.BeginTransaction())
            {
                _tokenRepository.RemoveByRefreshToken(refreshToken);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
