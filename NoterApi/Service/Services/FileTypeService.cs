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
    public interface IFileTypeService
    {
        public Task<FileType> GetById(string id);
        Task<IEnumerable<FileType>> GetAllAsync();

        public Task<Guid> AddAsync(FileType fileType);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(FileType item);
    }
    public class FileTypeService : IFileTypeService
    {
        private readonly IFileTypeRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public FileTypeService(IFileTypeRepository fileTypeRepository, IUnitOfWork unitOfWork)
        {
            _repository = fileTypeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> AddAsync(FileType fileType)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var result = await _repository.AddAsync(fileType);
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

        public async Task<IEnumerable<FileType>> GetAllAsync()
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

        public async Task<FileType> GetById(string id)
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

        public async Task UpdateAsync(FileType item)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                 await _repository.UpdateAsync(item);
                _unitOfWork.SaveChanges();
                return ;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }
}
