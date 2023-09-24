using Dake.DAL;
using Dake.Models;
using Dake.Models.ViewModels;
using Dake.Service.Common;
using Dake.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class BannerSevice : IBannerSevice
    {
        private readonly Context _context;
        private readonly IHostingEnvironment _environment;
        public BannerSevice(Context context, IHostingEnvironment environment)
        {
            _context = context;
            _environment = environment;

        }

        public async Task<BannerGetData> GetAllData(int page = 0)
        {
            IList<Banner> banners = await _context.Banner
                 .Skip(page)
                 .Take(10)
                 .Include(p => p.BannerImage)
                 .OrderByDescending(p => p.Id)
                 .ToListAsync();

            return new BannerGetData
            {
                banners = banners,
                bannersCount = banners.Count
            };
        }



        public async Task<Banner> GetBannerById(long id)
        {
            return await _context.Banner
                .Include(p=>p.BannerImage)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task Accepted(Banner dto)
        {
            try
            {
                var banner = _context.Banner.Find(dto.Id);

                banner.AcceptedDate = DateTime.Now;
                banner.AdminUserAccepted = dto.AdminUserAccepted;
                banner.adminConfirmStatus = dto.adminConfirmStatus;

               var res = _context.SaveChangesAsync().Result;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<long> AddOrUpdate(Banner dto, IList<IFormFile> files)
        {
            long id = 0;
            try
            {
                if (dto.Id > 0)
                {
                    _context.Banner.Update(dto);
                }
                else
                {
                    await _context.Banner.AddAsync(dto);
                }

                var res = await _context.SaveChangesAsync();

                
                if (res > 0)
                {
                    id = dto.Id;
                }

                await AddBannerFile(dto, files);
            }
            catch (Exception ex)
            {

                return id;
            }
           

            return id;
        }

        public async Task DeleteBanner(long id)
        {
            Banner banner = await _context.Banner.SingleOrDefaultAsync(p => p.Id == id);
            await DeleteBannerImages(banner.Id);

            _context.Banner.Remove(banner);
            await _context.SaveChangesAsync();
        }


        public async Task<ResultViewModels> AddBanner(Banner banner, IList<IFormFile> files)
        {    
            
            ResultViewModels result = new ResultViewModels();
      
            try
            {

                var setting = _context.Settings.FirstOrDefault();

                if (banner.user == null)
                {
                    result.Message = "لطفا از حساب کاربری خود خارج ، و مجددا وارد شوید";
                    result.IsSuccess = false;
                    return result;
                   
                }

                //////
                if (banner.user.IsBlocked)
                {
                    result.Message = "شما در لیست سیاه قرار دارید و مجاز به ثبت آگهی نمی باشید. ";
                    result.IsSuccess = false;
                    return result;
                }

               

                bool checkWrongWords = banner.title.Contains(setting.wrongWord);

                if (checkWrongWords)
                {
                    result.Message = "لطفا در توضیحات و عنوان از کلمات مناسب استفاده نمایید. ";
                    result.IsSuccess = false;
                    return result;
                }


                ////تایید خودکار آگهی //////////////////////
                if (setting.AutoAccept)
                {
                    banner.adminConfirmStatus = EnumStatus.Accept;

                    CommonService.SendSMS_Accept(banner.user.cellphone, banner.title);
                }

                var bannerId = AddOrUpdate(banner, files).Result;

                result.Data = bannerId;

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.IsSuccess = false;
                return result;
            }

            //result.Message = "لطفا از حساب کاربری خود خارج ، و مجددا وارد شوید";
            result.IsSuccess = true;
            return result;
        }


        public async Task DeleteBannerImage(long id)
        {
            var bannerImageCount =  _context.BannerImage.Where(w=> w.Id == id).Count();

            if (bannerImageCount > 1)
            {

                BannerImage bannerImage = await _context.BannerImage.SingleOrDefaultAsync(p => p.Id == id);

                var filePath = Path.Combine(_environment.WebRootPath, "Banner/", bannerImage.Name);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.BannerImage.Remove(bannerImage);
                await _context.SaveChangesAsync();
            }
        }

        private async Task DeleteBannerImages(long idBanner)
        {
            var bannerImages = await _context.BannerImage.Where(p => p.BannerId == idBanner).ToListAsync();

            foreach (var bannerImage in bannerImages)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "Banner/", bannerImage.Name);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                _context.BannerImage.Remove(bannerImage);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddBannerFile(Banner dto, IList<IFormFile> files)
        {
            if (files.Any())
            {
                foreach (var file in files)
                {
                    var namefile = Guid.NewGuid().ToString().Replace('-', '0').Substring(0, 7) + Path.GetExtension(file.FileName).ToLower();
                    var filePath = Path.Combine(_environment.WebRootPath, "Banner/", namefile);


                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {

                        await file.CopyToAsync(stream);
                    }


                    await _context.BannerImage.AddAsync(new BannerImage
                    {
                        BannerId = dto.Id,
                        FileLocation = "/Banner/" + namefile,
                        Name = namefile
                    });
                    await _context.SaveChangesAsync();

                }
            }
            else
            {
                await _context.BannerImage.AddAsync(new BannerImage
                {
                    BannerId = dto.Id,
                    FileLocation = "/images/nopic.jpg",
                    Name = "nopic"
                });
                await _context.SaveChangesAsync();
            }
        }


    }
}
