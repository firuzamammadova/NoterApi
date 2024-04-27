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
        Task<IEnumerable<RecordFile>> GetAllAsync(string userId);
        Task<IEnumerable<RecordFile>> GoBackGetChildren(string id, string userId);
        Task<IEnumerable<RecordFile>> GetAllStarredAsync(string userId);
        Task<IEnumerable<RecordFile>> SearchFiles(string key, string userId);

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync(string userId);
        public Task<Guid> AddAsync(RecordFile item);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
        Task AddContext(RecordFile item);

        Task UpdateLastOpenedDate(string id);
        Task ToggleStarFile(string id);
        Task<int> GetCountsOfChildren(string id);

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

        public async Task<IEnumerable<RecordFile>> GetAllAsync(string userId)
        {
            return await _query.GetAllAsync( userId);
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

        public async Task<IEnumerable<RecordFile>> GoBackGetChildren(string id, string userId)
        {
            return await _query.GoBackGetChildren(id,userId);
        }

        public async Task UpdateLastOpenedDate(string id)
        {
            await _command.UpdateLastOpenedDate(id);
        }

        public async Task ToggleStarFile(string id)
        {
            await _command.ToggleStarFile(id);  
        }

        public async Task<IEnumerable<RecordFile>> GetAllStarredAsync(string userId)
        {
            return await _query.GetAllStarredAsync(userId);
        }

        public async Task<IEnumerable<RecordFile>> GetRecentFilesAsync(string userId)
        {
            return await _query.GetRecentFilesAsync(userId);
        }

        public async Task<IEnumerable<RecordFile>> SearchFiles(string key, string userId)
        {
            return await _query.SearchFiles(key,userId);
        }

        public async Task<int> GetCountsOfChildren(string id)
        {
            return await _query.GetCountsOfChildren(id);
        }
    }
}
