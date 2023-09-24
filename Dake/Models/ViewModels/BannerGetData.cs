using System.Collections.Generic;

namespace Dake.Models.ViewModels
{
    public class BannerGetData
    {
        public IList<Banner> banners { get; set; }
        public int bannersCount { get; set; }
    }
}
