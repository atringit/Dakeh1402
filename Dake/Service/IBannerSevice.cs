using Dake.Models;
using Dake.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dake.Service
{
    public interface IBannerSevice
    {
        Task<long> AddOrUpdate(Banner dto, IList<IFormFile> files);
        Task Accepted(Banner dto);
        Task DeleteBanner(long id);
        Task DeleteBannerImage(long id);
        Task<BannerGetData> GetAllData(int page = 0, string search = "");
        Task<Banner> GetBannerById(long id);
        Task<ResultViewModels> AddBanner(Banner banner, IList<IFormFile> files);


    }
}