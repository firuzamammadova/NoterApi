using Core.Models;
using Dapper;
using Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CQRS.Queries
{
    public interface IFileTypeQuery
    {
        public Task<FileType> GetById(string id);
        Task<IEnumerable<FileType>> GetAllAsync();


    }

    public class FileTypeQuery : IFileTypeQuery
    {



        private readonly IUnitOfWork _unitOfWork;

        private readonly string _getAllSql = @"SELECT C.*  from dbo.FileTypes C
                                             WHERE C.DeleteStatus = 0";

        private readonly string _getByIdSql = @$"SELECT * FROM dbo.FileTypes WHERE Id=@id";

        public FileTypeQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<FileType>> GetAllAsync()
        {
            try
            {
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<FileType>(_getAllSql, null, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<FileType> GetById(string id)
        {
            try
            {
                var parameters = new
                {
                    id
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryFirstOrDefaultAsync<FileType>(_getByIdSql, parameters, _unitOfWork.GetTransaction());

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
