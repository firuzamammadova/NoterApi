using Core.Models;
using Microsoft.VisualBasic.FileIO;
using Repository.Infrastructure;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Service.Services
{
    public interface IRecordFileService
    {
        public Task<RecordFile> GetById(string id);
        Task<IEnumerable<RecordFile>> GetAllAsync();
        Task<IEnumerable<RecordFile>> GetAncestorFolders();
        Task<IEnumerable<RecordFile>> GetChildrenOfFolder(Guid parentId);
        Task<IEnumerable<RecordFile>> GoBackGetChildren(string id);
        Task<IEnumerable<RecordFile>> GetAllStarredAsync();

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync();

        public Task<Guid> AddAsync(RecordFile RecordFile);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
        Task AddContext(RecordFile item);


        Task UpdateLastOpenedDate(string id);
        Task ToggleStarFile(string id);

    }
    public class RecordFileService : IRecordFileService
    {
        private readonly IFileTypeRepository _typeRepository;

        private readonly IRecordFileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordFileService(IFileTypeRepository typeRepository, IRecordFileRepository repository, IUnitOfWork unitOfWork)
        {
            _typeRepository = typeRepository;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> AddAsync(RecordFile recordFile)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var typeId = await _typeRepository.GetIdByType(recordFile.Type);
                recordFile.TypeId = typeId;
                var result = await _repository.AddAsync(recordFile);
                _unitOfWork.SaveChanges();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await using var transaction = _unitOfWork.BeginTransaction();
            try
            {
                var res = await _repository.DeleteAsync(id);
                _unitOfWork.SaveChanges();
                return res;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public async Task<IEnumerable<RecordFile>> GetAllAsync()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IEnumerable<RecordFile>> GetAncestorFolders()
        {
            try
            {
                var result = await _repository.GetAllAsync();
                var ancestorFolders = result.Where(x => x.Type == FileTypeEnum.Folder && x.ParentId == null);
                return ancestorFolders;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IEnumerable<RecordFile>> GetChildrenOfFolder(Guid parentId)
        {
            try
            {
                var result = await _repository.GetAllAsync();
                var childrenOfFolder = result.Where(x => x.ParentId == parentId);
                return childrenOfFolder;

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
                var result = await _repository.GetById(id);
                return result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task UpdateAsync(RecordFile item)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                await _repository.UpdateAsync(item);
                _unitOfWork.SaveChanges();
                return;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public async Task AddContext(RecordFile item)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                await _repository.AddContext(item);
                _unitOfWork.SaveChanges();
                return;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }

        public async Task<IEnumerable<RecordFile>> GoBackGetChildren(string id)
        {
            try
            {
                var result = await _repository.GoBackGetChildren(id);
                if (result.Count() == 0)
                {
                   result=await GetAncestorFolders();
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task UpdateLastOpenedDate(string id)
        {
            await _repository.UpdateLastOpenedDate(id);
        }

        public async Task ToggleStarFile(string id)
        {
           await _repository.ToggleStarFile(id);
        }

        public async Task<IEnumerable<RecordFile>> GetAllStarredAsync()
        {
            try
            {
                var result = await _repository.GetAllStarredAsync();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<IEnumerable<RecordFile>> GetRecentFilesAsync()
        {
            try
            {
                var result = await _repository.GetRecentFilesAsync();
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
