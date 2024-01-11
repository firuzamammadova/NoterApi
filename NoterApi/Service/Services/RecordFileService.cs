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


        public Task<Guid> AddAsync(RecordFile RecordFile);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
    }
    public class RecordFileService : IRecordFileService
    {
        private readonly IRecordFileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public RecordFileService(IRecordFileRepository RecordFileRepository, IUnitOfWork unitOfWork)
        {
            _repository = RecordFileRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> AddAsync(RecordFile RecordFile)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var result = await _repository.AddAsync(RecordFile);
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
                var ancestorFolders = result.Where(x => x.Type == FileTypeEnum.Folder && x.ParentId==null);
                return ancestorFolders;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public async Task<IEnumerable<RecordFile>> GetChildrenOfFolder( Guid parentId)
        {
            try
            {
            var result = await _repository.GetAllAsync();
            var childrenOfFolder=result.Where(x=>x.ParentId==parentId);
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
    }
}
