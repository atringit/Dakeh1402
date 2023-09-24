using Dake.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Utility
{
	public class GetNoticeCategoryImage
	{
		private static Context _context;
		public GetNoticeCategoryImage(Context context)
		{
            _context = context;
		}
        public string getCategoryImage(int catId)
        {
            var categoryItem = _context.Categorys.FirstOrDefault(s => s.id == catId);
            if (!categoryItem.parentCategoryId.HasValue)
            {
                return categoryItem.image;
            }
            var categoryItem2 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem.parentCategoryId);
            if (!categoryItem2.parentCategoryId.HasValue)
            {
                return categoryItem2.image;
            }
            var categoryItem3 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem2.parentCategoryId);
            if (!categoryItem3.parentCategoryId.HasValue)
            {
                return categoryItem3.image;
            }
            var categoryItem4 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem3.parentCategoryId);
            if (!categoryItem4.parentCategoryId.HasValue)
            {
                return categoryItem4.image;
            }
            var categoryItem5 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem4.parentCategoryId);
            if (!categoryItem5.parentCategoryId.HasValue)
            {
                return categoryItem5.image;
            }
            var categoryItem6 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem5.parentCategoryId);
            if (!categoryItem6.parentCategoryId.HasValue)
            {
                return categoryItem6.image;
            }
            var categoryItem7 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem6.parentCategoryId);
            if (!categoryItem7.parentCategoryId.HasValue)
            {
                return categoryItem7.image;
            }
            var categoryItem8 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem7.parentCategoryId);
            if (!categoryItem8.parentCategoryId.HasValue)
            {
                return categoryItem8.image;
            }
            var categoryItem9 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem8.parentCategoryId);
            if (!categoryItem9.parentCategoryId.HasValue)
            {
                return categoryItem9.image;
            }
            var categoryItem10 = _context.Categorys.FirstOrDefault(s => s.id == categoryItem9.parentCategoryId);
            if (!categoryItem10.parentCategoryId.HasValue)
            {
                return categoryItem10.image;
            }
            return string.Empty;
        }





    }
}
