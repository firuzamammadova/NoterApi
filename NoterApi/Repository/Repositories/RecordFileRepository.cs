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

    public interface IRecordFileRepository
    {
        public Task<RecordFile> GetById(string id);
        Task<IEnumerable<RecordFile>> GetAllAsync();
        Task<IEnumerable<RecordFile>> GoBackGetChildren(string id);
        Task<IEnumerable<RecordFile>> GetAllStarredAsync();
        Task<IEnumerable<RecordFile>> SearchFiles(string key);

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync();
        public Task<Guid> AddAsync(RecordFile item);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
        Task AddContext(RecordFile item);

        Task UpdateLastOpenedDate(string id);
        Task ToggleStarFile(string id);
    }
    public class RecordFileRepository : IRecordFileRepository
    {
        private readonly IRecordFileCommand _command;
        private readonly IRecordFileQuery _query;

        public RecordFileRepository(IRecordFileCommand RecordFileCommand, IRecordFileQuery RecordFileQuery)
        {
            _command = RecordFileCommand;
            _query = RecordFileQuery;
        }

        public async Task<RecordFile> GetById(string id)
        {
            return await _query.GetById(id);
        }

        public async Task<Guid> AddAsync(RecordFile item)
        {
            return await _command.AddAsync(item);
        }

        public async Task<IEnumerable<RecordFile>> GetAllAsync()
        {
            return await _query.GetAllAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _command.DeleteAsync(id);
        }

        public async Task UpdateAsync(RecordFile item)
        {
            await _command.UpdateAsync(item);
            return;
        }

        public async Task AddContext(RecordFile item)
        {
            await _command.AddContext(item);
            return;
        }

        public async Task<IEnumerable<RecordFile>> GoBackGetChildren(string id)
        {
            return await _query.GoBackGetChildren(id);
        }

        public async Task UpdateLastOpenedDate(string id)
        {
            await _command.UpdateLastOpenedDate(id);
        }

        public async Task ToggleStarFile(string id)
        {
            await _command.ToggleStarFile(id);  
        }

        public async Task<IEnumerable<RecordFile>> GetAllStarredAsync()
        {
            return await _query.GetAllStarredAsync();
        }

        public async Task<IEnumerable<RecordFile>> GetRecentFilesAsync()
        {
            return await _query.GetRecentFilesAsync();
        }

        public async Task<IEnumerable<RecordFile>> SearchFiles(string key)
        {
            return await _query.SearchFiles(key);
        }
    }
}
