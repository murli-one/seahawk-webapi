using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Microsoft.AspNetCore.Http;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface INewsService
    {
        Task<IEnumerable<NewsFeed>> GetAllAsync();
        Task<IEnumerable<NewsFeed>> GetActiveFeedsAsync();
        Task<NewsFeed?> GetByIdAsync(int id);
        Task AddAsync(NewsFeed newsFeed);
        Task UpdateAsync(NewsFeed newsFeed);
        Task UpdateAsync(NewsFeed newsFeed, IFormFile? file);
        Task DeleteAsync(int id);
        Task SoftDeleteAsync(NewsFeed newsFeed);
        Task AddNewsAsync(NewsFeed news, IFormFile uploadfile);
        
    }
}