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
        Task<IEnumerable<RecordFile>> SearchFiles(string key);


        Task<IEnumerable<RecordFile>> GetAllStarredAsync();

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync();

    }

    public class RecordFileQuery : IRecordFileQuery
    {

        private readonly IUnitOfWork _unitOfWork;

        private readonly string _getAllSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0";
        private readonly string _getRecentFilesSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0 AND T.Type=1 ORDER BY LastOpenedDate DESC";
        private readonly string _getAllStarredSql = @"SELECT C.*, T.Type  from dbo.RecordFiles C 
                                               Left JOIN FileTypes  T ON T.Id=C.TypeId  
                                               WHERE C.DeleteStatus = 0 AND Starred=1";

        private readonly string _getByIdSql = @$"SELECT * FROM dbo.RecordFiles WHERE Id=@id";

        private readonly string _goBackGetChildrenSql = @$"SELECT * FROM RecordFiles 
                                                           WHERE ParentId=(SELECT TOP 1 ParentId FROM RecordFiles WHERE Id=@id) 
                                                           AND DeleteStatus=0";

        private readonly string _searchFilesSql = $@"DECLARE @SearchText NVARCHAR(MAX)
                                                     SET @SearchText = N'%' + @SEARCH + '%'
                                                     SELECT C.*, T.Type  from dbo.RecordFiles C 
                                                     Left JOIN FileTypes  T ON T.Id=C.TypeId 
                                                     WHERE C.DeleteStatus = 0 AND 
                                                     (C.Name LIKE @SearchText OR C.Context LIKE @SearchText)";
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

        public async Task<IEnumerable<RecordFile>> GetAllStarredAsync()
        {
            try
            {
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getAllStarredSql, null, _unitOfWork.GetTransaction());
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

        public async Task<IEnumerable<RecordFile>> GetRecentFilesAsync()
        {
            try
            {
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<RecordFile>(_getRecentFilesSql, null, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                throw e;
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

        public async Task<IEnumerable<RecordFile>> SearchFiles(string key)
        {
            try
            {
                var parameters = new
                {
                    search=key
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
