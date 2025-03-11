using Microsoft.EntityFrameworkCore;
using Xorate.Data;
using Xorate.Interfaces;
using Xorate.Models.Transfers;

namespace Xorate.Repository
{
    public class ServiceRepository : IService
    {
        private readonly ApplicationContext _context;

        public ServiceRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<BrokenImage>> GetPostImagesAsync()
        {
            return await _context.Posts.Select(e => new BrokenImage
            {
                Id = e.Id,
                Path = e.Path,
                ImageLink = e.Image
            }).ToListAsync();
        }

        public async Task<IEnumerable<BrokenImage>> GetShortPostImagesAsync()
        {
            return await _context.ShortPosts.Select(e => new BrokenImage
            {
                Id = e.Id,
                Path = e.Path,
                ImageLink = e.ImageLink
            }).ToListAsync();
        }
    }
}
