﻿using Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualBasic.FileIO;
using Repository.Infrastructure;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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
        Task<IEnumerable<RecordFile>> SearchFiles(string key);

        Task<IEnumerable<RecordFile>> GetRecentFilesAsync();

        public Task<Guid> AddAsync(RecordFile RecordFile);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateAsync(RecordFile item);
        Task AddContext(RecordFile item);
        string? getUserId();

        Task UpdateLastOpenedDate(string id);
        Task ToggleStarFile(string id);
       void setIHttpContextAccessor(IHttpContextAccessor httpContextAccessor);
    }
    public class RecordFileService : IRecordFileService
    {
        private readonly IFileTypeRepository _typeRepository;

        private readonly IRecordFileRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private  IHttpContextAccessor _httpContextAccessor;

        public RecordFileService(IFileTypeRepository typeRepository, IRecordFileRepository repository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _typeRepository = typeRepository;
            _repository = repository;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
       public void setIHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Guid> AddAsync(RecordFile recordFile)
        {
            await using var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var typeId = await _typeRepository.GetIdByType(recordFile.Type);
                recordFile.TypeId = typeId;
                var userId = getUserId();
                recordFile.UserId = new Guid(userId);
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
        public string? getUserId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            var userId = user?.Identity.GetUserId();
            return userId;
        }
        public async Task<IEnumerable<RecordFile>> GetAllAsync()
        {
            try
            {
                var id = getUserId();
                var result = await _repository.GetAllAsync(id);
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
                var userId = getUserId();

                var result = await _repository.GetAllAsync(userId);
                var ancestorFolders = result.Where(x =>  x.ParentId == null);
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
                var userId = getUserId();
                var result = await _repository.GetAllAsync(userId);
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
                var userId = getUserId();

                var result = await _repository.GoBackGetChildren(id, userId);
                if (result.Count() == 0)
                {
                    result = await GetAncestorFolders();
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
                var id = getUserId();
                var result = await _repository.GetAllStarredAsync(id);
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
                var id = getUserId();

                var result = await _repository.GetRecentFilesAsync(id);
                return result;
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
                var id = getUserId();

                var result = await _repository.SearchFiles(key, id);
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
