using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class ApplicationUserService : IApplicationUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ApplicationUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        {
            return await _unitOfWork.User.GetAllAsync();
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersInsteadOfYours(string userName)
        {
            return await _unitOfWork.User.GetAllAsync(x => x.UserName.ToLower() != userName.ToLower() && x.IsDeleted == false);
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _unitOfWork.User.GetByIdAsync(id);
        }
        public async Task AddAsync(ApplicationUser user)
        {
            await _unitOfWork.User.AddAsync(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);
            if (user != null)
            {
                await _unitOfWork.User.RemoveAsync(user);
                await _unitOfWork.SaveAsync();
            }
        }
        public Task<bool> UserExistsByUserNameAsync(string userName)
             => _unitOfWork.User.ExistsByUserNameAsync(userName);
        public Task<ApplicationUser> GetUserByUserNameAsync(string username)
             => _unitOfWork.User.GetByUserNameAsync(username);
    }
}
