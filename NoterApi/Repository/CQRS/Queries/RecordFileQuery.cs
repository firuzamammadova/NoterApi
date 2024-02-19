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
    public interface IRecordFileQuery
    {
        public Task<RecordFile> GetById(string id);
        Task<IEnumerable<RecordFile>> GetAllAsync();
        Task<IEnumerable<RecordFile>> GoBackGetChildren(string id);

    }

    public class RecordFileQuery : IRecordFileQuery
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly string _getAllSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0";

        private readonly string _getByIdSql = @$"SELECT * FROM dbo.RecordFiles WHERE Id=@id";

        private readonly string _goBackGetChildrenSql = @$"SELECT * FROM RecordFiles 
                                                           WHERE ParentId=(SELECT TOP 1 ParentId FROM RecordFiles 
                                                           WHERE Id=@id) AND DeleteStatus=0";


        public RecordFileQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RecordFile>> GetAllAsync()
        {
            try
            {
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getAllSql, null, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<RecordFile> GetById(string id)
        {
            try
            {
                var parameters = new
                {
                    id
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryFirstOrDefaultAsync<RecordFile>(_getByIdSql, parameters, _unitOfWork.GetTransaction());

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<RecordFile>> GoBackGetChildren(string id)
        {
            try
            {
                var parameters = new
                {
                    id
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_goBackGetChildrenSql, parameters, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
