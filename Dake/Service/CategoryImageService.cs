using Dake.DAL;
using Dake.Service.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dake.Service
{
    public class CategoryImageService : ICategoryImageService
    {
        private readonly Context _context;

        public CategoryImageService(Context context)
        {
            _context = context;
        }

        public async Task<string> GetCategoryImageAsync(int categoryId)
        {
            var currentCategoryId = categoryId;

            while (true)
            {
                var categoryItem = await _context.Categorys.FirstOrDefaultAsync(s => s.id == currentCategoryId);
                if (categoryItem == null)
                {
                    return string.Empty;
                }

                if (!categoryItem.parentCategoryId.HasValue)
                {
                    return categoryItem.image;
                }

                currentCategoryId = categoryItem.parentCategoryId.Value;
            }
        }
    }
}