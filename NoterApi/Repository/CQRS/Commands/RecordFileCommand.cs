using Core.Models;
using Dapper;
using Repository.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.CQRS.Commands
{
    public interface IRecordFileCommand
    {
        public Task<Guid> AddAsync(RecordFile item);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
        Task AddContext(RecordFile item);

    }
    public class RecordFileCommand : IRecordFileCommand
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _addSql = @$"INSERT INTO dbo.RecordFiles (Name,IsParent,TypeId,UserId,ParentId,Context,CreatedDate) 
                                          OUTPUT inserted.Id VALUES ( @{nameof(RecordFile.Name)},@{nameof(RecordFile.IsParent)},
                                          @{nameof(RecordFile.TypeId)},@{nameof(RecordFile.UserId)},@{nameof(RecordFile.ParentId)},@{nameof(RecordFile.Context)}, GetDate())";

        private readonly string _updateSql = $@"UPDATE dbo.RecordFiles SET Name=@{nameof(RecordFile.Name)},
                                                IsParent=@{nameof(RecordFile.IsParent)},
                                                ParentId=@{nameof(RecordFile.ParentId)},
                                                TypeId=@{nameof(RecordFile.TypeId)},
                                                UserId=@{nameof(RecordFile.UserId)},
                                                Context=@{nameof(RecordFile.Context)},
                                                CreatedDate= GetDate() 
                                                WHERE Id=@Id";

        private readonly string _addContextSql = $@"UPDATE dbo.RecordFiles SET Context=@{nameof(RecordFile.Context)} WHERE Id=@id ";


        private readonly string _deleteSql = $@"UPDATE dbo.RecordFiles SET DeleteStatus = 1 WHERE Id=@id ";

        public RecordFileCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> AddAsync(RecordFile item)
        {
            try
            {
                var result = await _unitOfWork.GetConnection()
                    .QuerySingleAsync<Guid>(_addSql, item, _unitOfWork.GetTransaction());
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }



        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var result = await _unitOfWork.GetConnection().QueryFirstOrDefaultAsync<bool>(_deleteSql, new { id }, _unitOfWork.GetTransaction());
                return true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task AddContext(RecordFile item)
        {
            try
            {
                await _unitOfWork.GetConnection()
                    .QueryAsync(_addContextSql, item, _unitOfWork.GetTransaction());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task UpdateAsync(RecordFile item)
        {
            try
            {
                await _unitOfWork.GetConnection()
                    .QueryAsync(_updateSql, item, _unitOfWork.GetTransaction());
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
