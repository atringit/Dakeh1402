using System.Threading.Tasks;

namespace Dake.Service.Interface
{
    public interface ICategoryImageService
    {
        Task<string> GetCategoryImageAsync(int categoryId);
    }
}
