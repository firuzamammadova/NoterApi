using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Models;
using Microsoft.AspNetCore.Identity;
using Repository.Identity;
using Repository.Infrastructure;

namespace Service.Identity
{
    public interface IRoleService
    {
        IQueryable<Role> GetAll();
        Task<Role> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task<Role> UpdateAsync(Role role);
        Task<IdentityResult> DeleteAsync(int roleId);
        Task<Role> FindByIdAsync(string id);
        Task<Role> FindByNameAsync(string name);
        Task<Role> FindUniqueByNameAsync(string normalizedUserName, string roleId);
        List<Role> GetAllForUI(int groupId);
    }

    public class RoleService : IRoleService
    {
        private readonly RoleManager _roleManager;


        public RoleService(RoleManager roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Role> CreateAsync(Role role)
        {
            if (await _roleManager.RoleExistsAsync(role.Name))
            {
                return null;
            }
            await _roleManager.CreateAsync(role);
            var result = await _roleManager.FindByNameAsync(role.Name);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(int roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            try
            {
                var result = await _roleManager.DeleteAsync(role);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Role> UpdateAsync(Role role)
        {
            await _roleManager.UpdateAsync(role);
            var result = await _roleManager.FindByNameAsync(role.Name);
            return result;
        }

        public async Task<Role> FindByIdAsync(string id)
        {
            var result = await _roleManager.FindByIdAsync(id);
            return result;
        }

        public async Task<Role> FindByNameAsync(string name)
        {
            var result = await _roleManager.FindByNameAsync(name);
            return result;
        }

        public IQueryable<Role> GetAll()
        {
            var result = _roleManager.Roles;
            return result;
        }


        public List<Role> GetAllForUI(int groupId)
        {
            var result = _roleManager.Roles;
            var listResult = new List<Role>(result);

            var MyDataIQ = listResult.Where(x => x.GroupId == groupId).OrderByDescending(x => x.Id);
            var groupList = MyDataIQ.ToList();

            return groupList;
        }

        public async Task<Role> GetByIdAsync(int id)
        {
            var result = await _roleManager.FindByIdAsync(id.ToString());
            return result;
        }

        public async Task<Role> FindUniqueByNameAsync(string normalizedUserName, string roleId)
        {
            var result = await _roleManager.FindUniqueByNameAsync(normalizedUserName, roleId);
            return result;
        }
    }

}
