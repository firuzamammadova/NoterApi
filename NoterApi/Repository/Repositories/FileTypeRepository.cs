using Core.Models;
using Repository.CQRS.Commands;
using Repository.CQRS.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{

    public interface IFileTypeRepository
    {
        public Task<FileType> GetById(string id);
        Task<IEnumerable<FileType>> GetAllAsync();
        public Task<Guid> GetIdByType(FileTypeEnum type);


        public Task<Guid> AddAsync(FileType item);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(FileType item);
    }
    public class FileTypeRepository : IFileTypeRepository
    {
        private readonly IFileTypeCommand _command;
        private readonly IFileTypeQuery _query;

        public FileTypeRepository(IFileTypeCommand fileTypeCommand, IFileTypeQuery fileTypeQuery)
        {
            _command = fileTypeCommand;
            _query = fileTypeQuery;
        }

        public async Task<FileType> GetById(string id)
        {
            return await _query.GetById(id);
        }

        public async Task<Guid> AddAsync(FileType item)
        {
            return await _command.AddAsync(item);
        }

        public async Task<IEnumerable<FileType>> GetAllAsync()
        {
            return await _query.GetAllAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _command.DeleteAsync(id);
        }

        public async Task UpdateAsync(FileType item)
        {
             await _command.UpdateAsync(item);
            return;
        }

        public async Task<Guid> GetIdByType(FileTypeEnum type)
        {
            return await _query.GetIdByType(type);
        }
    }
}
