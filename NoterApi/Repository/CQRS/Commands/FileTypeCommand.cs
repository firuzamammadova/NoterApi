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
    public interface IFileTypeCommand
    {
        public Task<Guid> AddAsync(FileType item);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(FileType item);
    }
    public class FileTypeCommand :IFileTypeCommand
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _addSql = @$"INSERT INTO dbo.FileTypes (Type) 
                                          OUTPUT inserted.Id VALUES ( @{nameof(FileType.Type)})";

        private readonly string _updateSql = $@"UPDATE dbo.FileTypes SET Type=@{nameof(FileType.Type)} WHERE Id=@Id";

        private readonly string _deleteSql = $@"UPDATE dbo.FileTypes SET DeleteStatus = 1 WHERE Id=@id ";

        public FileTypeCommand(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> AddAsync(FileType item)
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

        public async Task UpdateAsync(FileType item)
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
