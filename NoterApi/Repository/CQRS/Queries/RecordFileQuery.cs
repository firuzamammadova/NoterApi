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
        Task<IEnumerable<RecordFile>> GetAllAsync(string userId);
        Task<IEnumerable<RecordFile>> GoBackGetChildren(string id, string userId);
        Task<IEnumerable<RecordFile>> SearchFiles(string key, string userId);


        Task<IEnumerable<RecordFile>> GetAllStarredAsync(string userId);

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync(string userId);

    }

    public class RecordFileQuery : IRecordFileQuery
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly string _getAllSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0 AND C.UserId=@userId";
        private readonly string _getRecentFilesSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0 AND T.Type=1 AND C.UserId=@userId ORDER BY LastOpenedDate DESC";
        private readonly string _getAllStarredSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0 AND C.Starred=1 AND C.UserId=@userId";

        private readonly string _getByIdSql = @$"SELECT * FROM dbo.RecordFiles WHERE Id=@id";

        private readonly string _goBackGetChildrenSql = @$"SELECT * FROM RecordFiles 
                                                           WHERE ParentId=(SELECT TOP 1 ParentId FROM RecordFiles WHERE Id=@id) 
                                                           AND DeleteStatus=0 AND UserId=@userId";

        private readonly string _searchFilesSql = $@"DECLARE @SearchText NVARCHAR(MAX)
                                                     SET @SearchText = N'%' + @SEARCH + '%'
                                                     SELECT C.*, T.Type  from dbo.RecordFiles C 
                                                     Left JOIN FileTypes  T ON T.Id=C.TypeId 
                                                     WHERE C.DeleteStatus = 0 AND C.UserId=@userId AND 
                                                     (C.Name LIKE @SearchText OR C.Context LIKE @SearchText)";
        public RecordFileQuery(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RecordFile>> GetAllAsync(string userId)
        {
            try
            {
                var parameters = new
                {
                    userId
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getAllSql, parameters, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<RecordFile>> GetAllStarredAsync(string userId)
        {
            try
            {
                var parameters = new
                {
                    userId
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getAllStarredSql, parameters, _unitOfWork.GetTransaction());
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

        public async Task<IEnumerable<RecordFile>> GetRecentFilesAsync(string userId)
        {
            try
            {
                var parameters = new
                {
                    userId
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getRecentFilesSql, parameters, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<RecordFile>> GoBackGetChildren(string id,string userId)
        {
            try
            {
                var parameters = new
                {
                    id=id,
                    userId=userId
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

        public async Task<IEnumerable<RecordFile>> SearchFiles(string key, string userId)
        {
            try
            {
                var parameters = new
                {
                    search=key,
                    userId=userId
                };
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_searchFilesSql, parameters, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
