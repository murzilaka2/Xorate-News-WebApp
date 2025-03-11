using Xorate.Models.Transfers;

namespace Xorate.Interfaces
{
    public interface IService
    {
        Task<IEnumerable<BrokenImage>> GetPostImagesAsync();
        Task<IEnumerable<BrokenImage>> GetShortPostImagesAsync();
    }
}
