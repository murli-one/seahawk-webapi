using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHost;
        public NewsService(IUnitOfWork unitOfWork, IWebHostEnvironment webHost)
        {
            _unitOfWork = unitOfWork;
            _webHost = webHost;
        }
        public async Task AddAsync(NewsFeed news)
        {
            try
            {
                await _unitOfWork.News.AddAsync(news);
                await _unitOfWork.SaveAsync();
            }
            catch(Exception ex)
            {

            }

        }

        public async Task AddNewsAsync(NewsFeed news, IFormFile file)
        {
   
            //if (file != null) {
            //    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            //    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            //    if (!allowed.Contains(ext))
            //        throw new InvalidOperationException("Unsupported image type.");

            //    var uploadsFolder = Path.Combine(_webHost.WebRootPath, "NewsFeed");
            //    Directory.CreateDirectory(uploadsFolder);

            //    var baseName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "");
            //    var fileName = $"{baseName}_{Guid.NewGuid():N}{ext}";
            //    var filePath = Path.Combine(uploadsFolder, fileName);

            //    await using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            //    {
            //        await file.CopyToAsync(stream);
            //    }

            //    news.BannerImage = "/NewsFeed/" + fileName;
            //}
            //if (news.BannerImage == null)
            //    news.BannerImage = "";
            news.PublishDate = DateTime.Now;

            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.SaveAsync();
        }



        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NewsFeed>> GetActiveNewsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NewsFeed>> GetAllAsync()
        {
            return await _unitOfWork.News.GetAllAsync();
        }

        public async Task<NewsFeed?> GetByIdAsync(int id)
        {
            return await _unitOfWork.News.GetByIdAsync(id);
        }

        public async Task SoftDeleteAsync(NewsFeed newsFeed)
        {
            newsFeed.Status = Status.Inactive;
            await _unitOfWork.News.UpdateAsync(newsFeed);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(NewsFeed newsFeed)
        {
            await _unitOfWork.News.UpdateAsync(newsFeed);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateAsync(NewsFeed newsFeed, IFormFile? file)
        {
            var existing = await _unitOfWork.News.GetByIdAsync(newsFeed.Id);
            if (existing == null) throw new InvalidOperationException("News not found.");

            existing.Title = newsFeed.Title;
            existing.ShortDescription = newsFeed.ShortDescription;
            existing.Content = newsFeed.Content;
            existing.Status = newsFeed.Status;
            existing.PublishDate = newsFeed.PublishDate;

            //if (file != null && file.Length > 0)
            //{
            //    var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            //    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            //    if (!allowed.Contains(ext))
            //        throw new InvalidOperationException("Unsupported image type.");

            //    var webRoot = string.IsNullOrWhiteSpace(_webHost.WebRootPath)
            //        ? Path.Combine(_webHost.ContentRootPath, "wwwroot")
            //        : _webHost.WebRootPath;

            //    var folder = Path.Combine(webRoot, "NewsFeed");
            //    Directory.CreateDirectory(folder);

            //    var baseName = Path.GetFileNameWithoutExtension(file.FileName).Replace(" ", "");
            //    var isBig = file.Length > 1024 * 1024; 
            //    var targetExt = isBig ? ".jpg" : ext;
            //    var fileName = $"{baseName}_{Guid.NewGuid():N}{targetExt}";
            //    var newPath = Path.Combine(folder, fileName);

            //    if (isBig)
            //    {
            //        using var src = file.OpenReadStream();
            //        using var image = await Image.LoadAsync(src);
            //        var encoder = new JpegEncoder { Quality = 70 };
            //        await image.SaveAsJpegAsync(newPath, encoder);
            //    }
            //    else
            //    {
            //        await using var fs = new FileStream(newPath, FileMode.Create, FileAccess.Write, FileShare.None);
            //        await file.CopyToAsync(fs);
            //    }
     
            //    if (!string.IsNullOrWhiteSpace(existing.BannerImage))
            //    {
            //        var relative = existing.BannerImage.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            //        var oldPath = Path.Combine(webRoot, relative);
            //        try { if (File.Exists(oldPath)) File.Delete(oldPath); } catch { }
            //    }

            //    existing.BannerImage = "/NewsFeed/" + fileName;
            //}

            await _unitOfWork.News.UpdateAsync(existing);
            await _unitOfWork.SaveAsync();
        }
        public async Task<IEnumerable<NewsFeed>> GetActiveFeedsAsync()
        {
            var records = await _unitOfWork.News.GetActiveNews();
            return records;

        }

        //public async Task<IEnumerable<>> GetAllAsync()
        //{
        //    return await _unitOfWork.VesselDetail.GetAllAsync();
        //}

        //public async Task<VesselDetail?> GetByIdAsync(int id)
        //{
        //    return await _unitOfWork.VesselDetail.GetByIdAsync(id);
        //}

        //public async Task AddAsync(VesselDetail vesseldetail)
        //{
        //    await _unitOfWork.VesselDetail.AddAsync(vesseldetail);
        //    await _unitOfWork.SaveAsync();
        //}

        //public async Task UpdateAsync(VesselDetail vesseldetail)
        //{
        //    await _unitOfWork.VesselDetail.UpdateAsync(vesseldetail);
        //    await _unitOfWork.SaveAsync();
        //}

        //public async Task DeleteAsync(int id)
        //{
        //    var analysisResult = await _unitOfWork.VesselDetail.GetByIdAsync(id);
        //    if (analysisResult != null)
        //    {
        //        await _unitOfWork.VesselDetail.RemoveAsync(analysisResult);
        //        await _unitOfWork.SaveAsync();
        //    }
        //}
    }
}
